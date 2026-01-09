using System.ComponentModel.DataAnnotations;

namespace FAQq.Models
{
    public class Answer
    {
        public int Id { get; set; }

        [Required]
        [MinLength(10)]
        public string Content { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public int QuestionId { get; set; }
        public Question? Question { get; set; }

        public string UserId { get; set; }
        public ApplicationUser? User { get; set; }
    }
}
