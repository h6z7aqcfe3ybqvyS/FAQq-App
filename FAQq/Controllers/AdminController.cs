using FAQq.Data;
using FAQq.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FAQq.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(ApplicationDbContext context,
                               UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // DASHBOARD (raporty)
        public async Task<IActionResult> Index()
        {
            var categoriesWithoutAnswers = await _context.Categories
                .Select(c => new
                {
                    Category = c.Name,
                    Unanswered = c.Questions.Count(q => q.Answers.Count == 0 && q.IsApproved)
                })
                .OrderByDescending(x => x.Unanswered)
                .ToListAsync();

            var activeExperts = await _context.Users
                .Select(u => new
                {
                    User = u.Email,
                    Answers = u.Answers.Count
                })
                .OrderByDescending(x => x.Answers)
                .ToListAsync();

            ViewBag.Categories = categoriesWithoutAnswers;
            ViewBag.Experts = activeExperts;

            return View();
        }

        //  Lista użytkowników
        public async Task<IActionResult> Users()
        {
            var users = _userManager.Users.ToList();
            return View(users);
        }

        // Nadaj moderatora
        public async Task<IActionResult> MakeModerator(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                await _userManager.AddToRoleAsync(user, "Moderator");

                // DEBUG sprawdź role
                var roles = await _userManager.GetRolesAsync(user);
                TempData["Roles"] = string.Join(", ", roles);
            }

            return RedirectToAction(nameof(Users));
        }

        // Odebranie moderatora
        public async Task<IActionResult> RemoveModerator(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                // usuń Moderator
                await _userManager.RemoveFromRoleAsync(user, "Moderator");

                // jeśli nie ma żadnej roli dodaj User
                var roles = await _userManager.GetRolesAsync(user);
                if (!roles.Any())
                {
                    await _userManager.AddToRoleAsync(user, "User");
                }

                TempData["Roles"] = string.Join(", ", await _userManager.GetRolesAsync(user));
            }

            return RedirectToAction(nameof(Users));
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                return NotFound();

            // Admin nie może usunąć samego siebie
            var currentUser = await _userManager.GetUserAsync(User);
            if (user.Id == currentUser.Id)
                return BadRequest("Nie możesz usunąć własnego konta.");

            await _userManager.DeleteAsync(user);

            return RedirectToAction(nameof(Users));
        }



    }
}
