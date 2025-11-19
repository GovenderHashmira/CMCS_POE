using Microsoft.AspNetCore.Mvc;
using CMCS_POE.Data;
using CMCS_POE.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace CMCS_POE.Controllers
{
    public class CoordinatorController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public CoordinatorController(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> AllClaims()
        {
            // Get all claims with related lecturer and documents
            var claims = await _context.Claims
                .Include(c => c.Lecturer)
                .Include(c => c.DocumentUploads)
                .ToListAsync();

            return View(claims);
        }

        [HttpGet]
        public async Task<IActionResult> ClaimDetails(int id)
        {
            var claim = await _context.Claims
                .Include(c => c.Lecturer)
                .Include(c => c.DocumentUploads)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (claim == null)
                return NotFound();

            return View(claim);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var claim = await _context.Claims.FindAsync(id);
            if (claim == null)
                return NotFound();

            claim.Status = "Approved";
            claim.ApprovalDate = DateTime.Now;

            _context.Update(claim);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(AllClaims));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id)
        {
            var claim = await _context.Claims.FindAsync(id);
            if (claim == null)
                return NotFound();

            claim.Status = "Rejected";
            claim.ApprovalDate = DateTime.Now;

            _context.Update(claim);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(AllClaims));
        }
    }
}
