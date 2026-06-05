using KiemTraGiuaKy.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KiemTraGiuaKy.Controllers
{
    // Câu 4: /enroll/** -> chỉ STUDENT
    // Câu 6: Đăng ký học phần, hủy đăng ký
    // Câu 7: My Courses
    [Authorize(Roles = "Student")]
    public class EnrollController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public EnrollController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /Enroll/MyCourses - Câu 7
        public async Task<IActionResult> MyCourses()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var enrollments = await _context.Enrollments
                .Include(e => e.Course)
                    .ThenInclude(c => c!.Category)
                .Where(e => e.UserId == user.Id)
                .OrderByDescending(e => e.EnrollDate)
                .ToListAsync();

            return View(enrollments);
        }

        // POST: /Enroll/Enroll - Câu 6: Đăng ký học phần
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Enroll(int courseId, string? returnUrl = null)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            // Kiểm tra khóa học có tồn tại không
            var course = await _context.Courses.FindAsync(courseId);
            if (course == null)
            {
                TempData["ErrorMessage"] = "Course not found.";
                return RedirectToAction("Index", "Home");
            }

            // Kiểm tra đã đăng ký chưa
            var existing = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.UserId == user.Id && e.CourseId == courseId);

            if (existing != null)
            {
                TempData["WarningMessage"] = $"You are already enrolled in '{course.Name}'.";
                return RedirectToLocal(returnUrl);
            }

            var enrollment = new Enrollment
            {
                UserId = user.Id,
                CourseId = courseId,
                EnrollDate = DateTime.Now
            };

            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Successfully enrolled in '{course.Name}'!";
            return RedirectToLocal(returnUrl);
        }

        // POST: /Enroll/Unenroll - Câu 6: Hủy đăng ký
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unenroll(int enrollmentId, string? returnUrl = null)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var enrollment = await _context.Enrollments
                .Include(e => e.Course)
                .FirstOrDefaultAsync(e => e.Id == enrollmentId && e.UserId == user.Id);

            if (enrollment == null)
            {
                TempData["ErrorMessage"] = "Enrollment not found.";
                return RedirectToLocal(returnUrl);
            }

            var courseName = enrollment.Course?.Name ?? "the course";
            _context.Enrollments.Remove(enrollment);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Successfully unenrolled from '{courseName}'.";
            return RedirectToLocal(returnUrl);
        }

        // POST: /Enroll/UnenrollByCourse - Hủy từ trang Home
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnenrollByCourse(int courseId, string? returnUrl = null)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var enrollment = await _context.Enrollments
                .Include(e => e.Course)
                .FirstOrDefaultAsync(e => e.CourseId == courseId && e.UserId == user.Id);

            if (enrollment != null)
            {
                var courseName = enrollment.Course?.Name ?? "the course";
                _context.Enrollments.Remove(enrollment);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Successfully unenrolled from '{courseName}'.";
            }

            return RedirectToLocal(returnUrl);
        }

        private IActionResult RedirectToLocal(string? returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            return RedirectToAction("Index", "Home");
        }
    }
}
