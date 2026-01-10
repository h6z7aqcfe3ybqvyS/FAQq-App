using System.ComponentModel.DataAnnotations;

namespace FAQq.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        [Display(Name = "Nazwa kategorii")]
        public string Name { get; set; }

        // podkategorie
        public int? ParentCategoryId { get; set; }
        [Display(Name = "Nadrzędna kategoria")]
        public Category? ParentCategory { get; set; }

        public ICollection<Category> SubCategories { get; set; } = new List<Category>();
        public ICollection<Question> Questions { get; set; } = new List<Question>();
    }

}
