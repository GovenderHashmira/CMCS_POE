using CMCS_POE.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;

namespace CMCS_POE.Controllers
{
    [Authorize(Roles = "HR")] 
    public class HRController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public HRController(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var users = await _context.Users.ToListAsync();
            return View(users);
        }

        public async Task<IActionResult> UpdateUser(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        public IActionResult CreateUser()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(AppUser user, string password, string role)
        {
            if (!ModelState.IsValid)
            {
                return View(user);
            }

            if (string.IsNullOrWhiteSpace(user.UserName))
                user.UserName = user.Email?.Trim();

            user.EmailConfirmed = true;

            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(role))
                {
                    if (!await _userManager.IsInRoleAsync(user, role))
                    {
                        await _userManager.AddToRoleAsync(user, role);
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateUser(string id, AppUser updatedUser, string role)
        {
            if (id != updatedUser.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(updatedUser);
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            user.FirstName = updatedUser.FirstName;
            user.LastName = updatedUser.LastName;
            user.Email = updatedUser.Email;
            user.UserName = updatedUser.Email;
            user.HourlyRate = updatedUser.HourlyRate;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {

                var currentRoles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, currentRoles);

                if (!string.IsNullOrEmpty(role))
                {
                    await _userManager.AddToRoleAsync(user, role);
                }

                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(updatedUser);
        }

        [HttpGet]
        public async Task<IActionResult> GenerateClaimsReport()
        {
            // Get all claims (or filter by date if needed)
            var claims = await _context.Claims
                .Include(c => c.Lecturer)
                .Include(c => c.DocumentUploads)
                .ToListAsync();

            using (var stream = new MemoryStream())
            {
                Document doc = new Document(PageSize.A4, 25, 25, 30, 30);
                PdfWriter.GetInstance(doc, stream);
                doc.Open();

                // Add Title
                var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16);
                doc.Add(new Paragraph("Claims Report", titleFont));
                doc.Add(new Paragraph("Generated on: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm")));
                doc.Add(new Paragraph(" ")); // blank line

                // Add Table
                PdfPTable table = new PdfPTable(6);
                table.WidthPercentage = 100;
                table.SetWidths(new float[] { 10f, 15f, 15f, 15f, 20f, 25f });

                // Table Header
                var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
                table.AddCell(new PdfPCell(new Phrase("Claim ID", headerFont)));
                table.AddCell(new PdfPCell(new Phrase("Lecturer", headerFont)));
                table.AddCell(new PdfPCell(new Phrase("Hours Worked", headerFont)));
                table.AddCell(new PdfPCell(new Phrase("Hourly Rate", headerFont)));
                table.AddCell(new PdfPCell(new Phrase("Total Payment", headerFont)));
                table.AddCell(new PdfPCell(new Phrase("Status", headerFont)));

                // Table Data
                foreach (var claim in claims)
                {
                    table.AddCell(claim.Id.ToString());
                    table.AddCell($"{claim.Lecturer.FirstName} {claim.Lecturer.LastName}");
                    table.AddCell(claim.HoursWorked.ToString());
                    table.AddCell(claim.HourlyRate.ToString("C"));
                    table.AddCell(claim.TotalPayment.ToString("C"));
                    table.AddCell(claim.Status);
                }

                doc.Add(table);
                doc.Close();

                return File(stream.ToArray(), "application/pdf", "ClaimsReport.pdf");
            }
        }

    }
}
