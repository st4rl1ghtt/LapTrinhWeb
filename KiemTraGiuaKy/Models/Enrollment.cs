using System.ComponentModel.DataAnnotations;

namespace KiemTraGiuaKy.Models
{
    public class Enrollment
    {
        public int Id { get; set; }

        // FK -> AspNetUsers.Id
        [Required]
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }

        // FK -> Course.Id
        [Required]
        public int CourseId { get; set; }
        public Course? Course { get; set; }

        [Display(Name = "Enroll Date")]
        public DateTime EnrollDate { get; set; } = DateTime.Now;
    }
}
