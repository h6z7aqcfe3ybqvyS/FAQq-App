using FAQq.Data;
using FAQq.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace FAQq.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string search, int? categoryId)
        {
            var query = _context.Questions
                .Where(q => q.IsApproved)
                .Include(q => q.Category)
                .Include(q => q.User)
                .AsQueryable();

            // wyszukiwanie w tytule i treœci
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(q =>
                    q.Title.Contains(search) ||
                    q.Content.Contains(search));
            }

            // filtrowanie po kategorii
            if (categoryId.HasValue)
            {
                query = query.Where(q => q.CategoryId == categoryId.Value);
            }

            var allCategories = await _context.Categories.ToListAsync();
            ViewData["Categories"] = BuildCategoryTree(allCategories);

            // zapamiêtaj co by³o wybrane
            ViewData["Search"] = search;
            ViewData["SelectedCategory"] = categoryId;

            return View(await query.ToListAsync());
        }

        private List<SelectListItem> BuildCategoryTree(IEnumerable<Category> categories, int? parentId = null, string prefix = "")
        {
            return categories
                .Where(c => c.ParentCategoryId == parentId)
                .OrderBy(c => c.Name)
                .SelectMany(c => new[]
                {
            new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = prefix + c.Name
            }
                }.Concat(BuildCategoryTree(categories, c.Id, prefix + "— ")))
                .ToList();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
