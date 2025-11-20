using CMCS_POE.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using static System.Net.WebRequestMethods;


namespace CMCS_POE.Controllers
{
    [Authorize(Roles = "Lecturer")]
    public class LecturerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        public LecturerController(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var recentClaims = await _context.Claims
                .Where(c => c.LecturerId == user.Id)
                .Include(c => c.DocumentUploads)
                .OrderByDescending(c => c.SubmissionDate)
                .ToListAsync();

            return View(recentClaims);
        }

        public IActionResult SubmitClaim()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitClaim(Claim claim)
        {
            if (!ModelState.IsValid)
            {
                return View(claim);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            claim.DocumentUploads = new List<DocumentUpload>();

            claim.LecturerId = user.Id;
            claim.HourlyRate = user.HourlyRate;
            claim.SubmissionDate = DateTime.Now;
            claim.Status = "Pending";
            claim.TotalPayment = claim.HoursWorked * claim.HourlyRate;
            claim.DocumentName = null;
            claim.DocumentPath = null;

            _context.Claims.Add(claim);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch
            {
                ModelState.AddModelError("", "Unable to save claim. Please try again.");
                return View(claim);
            }

            if (claim.Document != null && claim.Document.Length > 0)
            {
                var allowedExtensions = new[] { ".pdf", ".docx", ".xlsx" };
                var extension = Path.GetExtension(claim.Document.FileName).ToLower();

                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("Document", "Invalid file type. Only PDF, DOCX, XLSX allowed.");
                    return View(claim);
                }

                if (claim.Document.Length > 5 * 1024 * 1024)
                {
                    ModelState.AddModelError("Document", "File size cannot exceed 5MB.");
                    return View(claim);
                }

                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = $"{Guid.NewGuid()}_{claim.Document.FileName}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await claim.Document.CopyToAsync(stream);
                }

                claim.DocumentName = claim.Document.FileName;
                claim.DocumentPath = "/uploads/" + uniqueFileName;

                var upload = new DocumentUpload
                {
                    ClaimId = claim.Id,
                    DocumentName = claim.Document.FileName,
                    DocumentPath = claim.DocumentPath,
                    UploadDate = DateTime.Now
                };

                _context.DocumentUploads.Add(upload);

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch 
                {
                    ModelState.AddModelError("", "Claim saved but document could not be uploaded.");
                    return View(claim);
                }
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
