using FAQq.Models;
using Microsoft.AspNetCore.Identity;

namespace FAQq.Models
{
    public class ApplicationUser : IdentityUser
    {
        // pytania zadane przez użytkownika
        public ICollection<Question> Questions { get; set; } = new List<Question>();

        // odpowiedzi dodane przez moderatora/eksperta
        public ICollection<Answer> Answers { get; set; } = new List<Answer>();
    }
}
