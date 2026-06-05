using System.ComponentModel.DataAnnotations;

namespace KiemTraGiuaKy.Models
{
    public class Course
    {
        public int Id { get; set; }

        [Required, StringLength(200)]
        [Display(Name = "Course Name")]
        public string Name { get; set; } = string.Empty;

        // Hình ảnh minh họa (đường dẫn)
        public string? Image { get; set; }

        [Required]
        [Range(1, 10)]
        [Display(Name = "Credits")]
        public int Credits { get; set; }

        [Required, StringLength(200)]
        [Display(Name = "Lecturer")]
        public string Lecturer { get; set; } = string.Empty;

        // Foreign key -> Category
        [Display(Name = "Category")]
        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        // Navigation property
        public List<Enrollment>? Enrollments { get; set; }
    }
}
