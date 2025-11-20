using CMCS_POE.Data;
using CMCS_POE.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace CMCS_POE.Controllers
{
    [Authorize(Roles = "Coordinator")]
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
            var claims = await _context.Claims.Include(c => c.Lecturer)
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
        public async Task<IActionResult> Approve(int claimId)
        {
            var claim = await _context.Claims.Include(c => c.Lecturer)
                                             .FirstOrDefaultAsync(c => c.Id == claimId);
            if (claim == null)
            {
                return NotFound();
            }

            claim.Status = "Approved";
            claim.ApprovalDate = DateTime.Now;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(AllClaims));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int claimId)
        {
            var claim = await _context.Claims.Include(c => c.Lecturer)
                                             .FirstOrDefaultAsync(c => c.Id == claimId);
            if (claim == null)
            {
                return NotFound();
            }

            claim.Status = "Rejected";
            claim.ApprovalDate = DateTime.Now;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(AllClaims));
        }

    }
}
