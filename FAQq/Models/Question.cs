using System.ComponentModel.DataAnnotations;
namespace FAQq.Models
{
    public class Question
    {
        public int Id { get; set; }

        [Required]
        [MinLength(10)]
        [MaxLength(200)]
        [Display(Name = "Tytuł")]
        public string Title { get; set; }

        [Required]
        [MinLength(20)]
        [MaxLength(600)]
        [Display(Name = "Treść pytania")]
        public string Content { get; set; }
        [Display(Name = "Data dodania")]

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        [Display(Name = "Zatwierdzone")]
        public bool IsApproved { get; set; } = false;
        
        public int CategoryId { get; set; }
        [Display(Name = "Kategoria")]
        public Category? Category { get; set; }
        [Display(Name = "Autor")]
        public string UserId { get; set; }
        public ApplicationUser? User { get; set; }

        public ICollection<Answer> Answers { get; set; } = new List<Answer>();
        public ICollection<QuestionTag> QuestionTags { get; set; } = new List<QuestionTag>();
    }

}
