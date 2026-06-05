using System.ComponentModel.DataAnnotations;

namespace KiemTraGiuaKy.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        [Display(Name = "Category Name")]
        public string Name { get; set; } = string.Empty;

        // Navigation property
        public List<Course>? Courses { get; set; }
    }
}
