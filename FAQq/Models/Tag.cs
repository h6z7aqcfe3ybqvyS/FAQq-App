using System.ComponentModel.DataAnnotations;

namespace FAQq.Models
{
    public class Tag
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        public ICollection<QuestionTag> QuestionTags { get; set; } = new List<QuestionTag>();
    }
}
