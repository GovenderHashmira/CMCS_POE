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

        public IActionResult Index()
        {
            return View();
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

            claim.LecturerId = user.Id;
            claim.HourlyRate = user.HourlyRate;
            claim.SubmissionDate = DateTime.Now;
            claim.Status = "Pending";

            claim.TotalPayment = claim.HoursWorked * claim.HourlyRate;

            if (claim.Document != null && claim.Document.Length > 0)
            {
                var allowedExtensions = new[] { ".pdf", ".docx", ".xlsx" };
                var extension = Path.GetExtension(claim.Document.FileName).ToLower();

                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("Document", "Invalid file type. Only PDF, DOCX, XLSX allowed.");
                    return View(claim);
                }

                if (claim.Document.Length > 5 * 1024 * 1024) // 5MB
                {
                    ModelState.AddModelError("Document", "File size cannot exceed 5MB.");
                    return View(claim);
                }

                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + claim.Document.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await claim.Document.CopyToAsync(fileStream);
                }

                claim.DocumentUploads.Add(new DocumentUpload
                {
                    DocumentName = claim.Document.FileName,
                    DocumentPath = "/uploads/" + uniqueFileName,
                    UploadDate = DateTime.Now
                });
            }

            _context.Claims.Add(claim);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
