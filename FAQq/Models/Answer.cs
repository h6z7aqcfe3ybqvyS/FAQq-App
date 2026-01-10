using System.ComponentModel.DataAnnotations;

namespace FAQq.Models
{
    public class Answer
    {
        public int Id { get; set; }

        [Required]
        [MinLength(10)]
        [Display(Name = "Treść odpowiedzi")]
        public string Content { get; set; }
        [Display(Name = "Data")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public int QuestionId { get; set; }
        public Question? Question { get; set; }
        [Display(Name = "Autor")]
        public string UserId { get; set; }
        public ApplicationUser? User { get; set; }
    }
}
