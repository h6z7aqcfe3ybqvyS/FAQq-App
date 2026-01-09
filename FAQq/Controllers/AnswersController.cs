using FAQq.Data;
using FAQq.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FAQq.Controllers
{
    [Authorize]
    public class AnswersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AnswersController(ApplicationDbContext context,
                                 UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> Create(int questionId, string content)
        {
            if (questionId == 0)
                throw new Exception("QUESTION ID = 0");

            if (string.IsNullOrWhiteSpace(content))
                throw new Exception("CONTENT IS EMPTY");

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                throw new Exception("USER IS NULL");

            var answer = new Answer
            {
                QuestionId = questionId,
                Content = content,
                UserId = user.Id
            };

            _context.Answers.Add(answer);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Questions", new { id = questionId });
        }

    }
}
