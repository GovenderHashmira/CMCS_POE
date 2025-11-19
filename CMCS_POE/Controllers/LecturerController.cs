using CMCS_POE.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CMCS_POE.Controllers
{
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
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            claim.LecturerId = user.Id;
            claim.HourlyRate = user.HourlyRate;
            claim.SubmissionDate = DateTime.Now;
            claim.Status = "Pending";

            if (claim.Document != null && claim.Document.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + claim.Document.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await claim.Document.CopyToAsync(fileStream);
                }

                claim.DocumentPath = "/uploads/" + uniqueFileName;
                claim.DocumentName = claim.Document.FileName;
            }

            _context.Claims.Add(claim);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
