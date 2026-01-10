using FAQq.Data;
using FAQq.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;


namespace FAQq.Controllers
{
    public class QuestionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public QuestionsController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        // GET: Questions
        public async Task<IActionResult> Index(string search, int? categoryId)
        {
            var query = _context.Questions
                .Where(q => q.IsApproved)
                .Include(q => q.Category)
                .Include(q => q.User)
                .AsQueryable();

            // wyszukiwanie w tytule i treści
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

            // zapamiętaj co było wybrane
            ViewData["Search"] = search;
            ViewData["SelectedCategory"] = categoryId;

            return View(await query.ToListAsync());
        }

        // METODA Pending()
        [Authorize(Roles = "Moderator,Admin")]
        public async Task<IActionResult> Pending()
        {
            var questions = await _context.Questions
                .Where(q => !q.IsApproved)
                .Include(q => q.Category)
                .Include(q => q.User)
                .ToListAsync();

            return View(questions);
        }


        // METODA Approve()
        [Authorize(Roles = "Moderator,Admin")]
        public async Task<IActionResult> Approve(int id)
        {
            var question = await _context.Questions.FindAsync(id);
            if (question == null) return NotFound();

            question.IsApproved = true;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Pending));
        }

        // GET: Questions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var question = await _context.Questions
                .Include(q => q.Category)
                .Include(q => q.User)
                .Include(q => q.Answers)
                    .ThenInclude(a => a.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (question == null)
                return NotFound();

            return View(question);
        }


        // GET: Questions/Create
        [Authorize]
        public async Task<IActionResult> Create()
        {
            var categories = await _context.Categories.ToListAsync();
            ViewData["Categories"] = BuildCategoryTree(categories);
            return View();
        }
        // POST: Questions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Content,CategoryId")] Question question)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                throw new Exception("USER IS NULL – AUTH BROKEN");

            question.UserId = user.Id;
            question.IsApproved = false;
            question.CreatedAt = DateTime.Now;

            _context.Questions.Add(question);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }



        // GET: Questions/Edit/5
        [Authorize(Roles = "Admin,Moderator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var question = await _context.Questions.FindAsync(id);
            if (question == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", question.CategoryId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", question.UserId);
            return View(question);
        }

        // POST: Questions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin,Moderator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Content,CreatedAt,IsApproved,CategoryId,UserId")] Question question)
        {
            if (id != question.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(question);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QuestionExists(question.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", question.CategoryId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", question.UserId);
            return View(question);
        }

        // GET: Questions/Delete/5
        [Authorize(Roles = "Admin,Moderator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var question = await _context.Questions
                .Include(q => q.Category)
                .Include(q => q.User)
                .Include(q => q.Answers)
                    .ThenInclude(a => a.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (question == null)
            {
                return NotFound();
            }

            return View(question);
        }

        // POST: Questions/Delete/5
        [Authorize(Roles = "Admin,Moderator")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var question = await _context.Questions.FindAsync(id);
            if (question != null)
            {
                _context.Questions.Remove(question);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool QuestionExists(int id)
        {
            return _context.Questions.Any(e => e.Id == id);
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

    }
}
