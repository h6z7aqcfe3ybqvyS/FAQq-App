using FAQq.Models;
using Microsoft.AspNetCore.Identity;

namespace FAQq.Models
{
    public class ApplicationUser : IdentityUser
    {
        // pytania
        public ICollection<Question> Questions { get; set; } = new List<Question>();

        // odpowiedzi
        public ICollection<Answer> Answers { get; set; } = new List<Answer>();
    }
}
