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
            Console.WriteLine("➡ ENTER SubmitClaim POST");

            if (!ModelState.IsValid)
            {
                Console.WriteLine("❌ ModelState is invalid");
                return View(claim);
            }

            // Get the currently logged-in user
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            // Initialize the DocumentUploads collection
            claim.DocumentUploads = new List<DocumentUpload>();

            // Assign user-related info
            claim.LecturerId = user.Id;
            claim.HourlyRate = user.HourlyRate;
            claim.SubmissionDate = DateTime.Now;
            claim.Status = "Pending";
            claim.TotalPayment = claim.HoursWorked * claim.HourlyRate;

            // Temporarily set document fields to null
            claim.DocumentName = null;
            claim.DocumentPath = null;

            // Add claim to database
            _context.Claims.Add(claim);
            Console.WriteLine("✔ Claim added to context");

            try
            {
                await _context.SaveChangesAsync();
                Console.WriteLine("✔ Claim saved to database, ID: " + claim.Id);
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Exception while saving claim: " + ex.Message);
                ModelState.AddModelError("", "Unable to save claim. Please try again.");
                return View(claim);
            }

            // Handle document upload, if any
            if (claim.Document != null && claim.Document.Length > 0)
            {
                Console.WriteLine("➡ Entering document upload block");

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

                // Save the file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await claim.Document.CopyToAsync(stream);
                }

                // Update claim fields for the document
                claim.DocumentName = claim.Document.FileName;
                claim.DocumentPath = "/uploads/" + uniqueFileName;

                // Create DocumentUpload record
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
                    Console.WriteLine("✔ DocumentUpload saved to database");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("❌ Exception while saving document: " + ex.Message);
                    ModelState.AddModelError("", "Claim saved but document could not be uploaded.");
                    return View(claim);
                }
            }

            Console.WriteLine("➡ EXIT SubmitClaim POST");
            return RedirectToAction(nameof(MyClaims));
        }




        public async Task<IActionResult> MyClaims()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var claims = await _context.Claims
                .Where(c => c.LecturerId == user.Id)
                .Include(c => c.DocumentUploads) 
                .OrderByDescending(c => c.SubmissionDate)
                .ToListAsync();

            return View(claims);
        }
    }
}
