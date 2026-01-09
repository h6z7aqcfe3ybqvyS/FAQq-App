using System.ComponentModel.DataAnnotations;
namespace FAQq.Models
{
    public class Question
    {
        public int Id { get; set; }

        [Required]
        [MinLength(10)]
        [MaxLength(200)]
        public string Title { get; set; }

        [Required]
        [MinLength(20)]
        public string Content { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public bool IsApproved { get; set; } = false;

        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        public string UserId { get; set; }
        public ApplicationUser? User { get; set; }

        public ICollection<Answer> Answers { get; set; } = new List<Answer>();
        public ICollection<QuestionTag> QuestionTags { get; set; } = new List<QuestionTag>();
    }

}
