using KiemTraGiuaKy.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KiemTraGiuaKy.Areas.Admin.Controllers
{
    // Câu 10: Dashboard thống kê cho Admin
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /Admin/Dashboard
        public async Task<IActionResult> Index()
        {
            // Câu 10: Tổng số học phần
            var totalCourses = await _context.Courses.CountAsync();

            // Câu 10: Tổng số sinh viên (users có role Student)
            var students = await _userManager.GetUsersInRoleAsync("Student");
            var totalStudents = students.Count;

            // Câu 10: Tổng số lượt đăng ký
            var totalEnrollments = await _context.Enrollments.CountAsync();

            // Thống kê top 5 khóa học được đăng ký nhiều nhất
            var topCoursesRaw = await _context.Enrollments
                .Include(e => e.Course)
                .GroupBy(e => e.Course!.Name)
                .Select(g => new CourseStatItem { CourseName = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(5)
                .ToListAsync();

            // Thống kê theo danh mục
            var categoryStatsRaw = await _context.Courses
                .Include(c => c.Category)
                .GroupBy(c => c.Category!.Name)
                .Select(g => new CategoryStatItem { CategoryName = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .ToListAsync();

            // Đăng ký gần đây (5 gần nhất)
            var recentEnrollments = await _context.Enrollments
                .Include(e => e.User)
                .Include(e => e.Course)
                .OrderByDescending(e => e.EnrollDate)
                .Take(5)
                .ToListAsync();

            ViewData["TotalCourses"] = totalCourses;
            ViewData["TotalStudents"] = totalStudents;
            ViewData["TotalEnrollments"] = totalEnrollments;
            ViewData["TopCourses"] = topCoursesRaw;
            ViewData["CategoryStats"] = categoryStatsRaw;
            ViewData["RecentEnrollments"] = recentEnrollments;

            return View();
        }
    }

    // Strongly-typed DTOs for Dashboard stats
    public class CourseStatItem
    {
        public string CourseName { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class CategoryStatItem
    {
        public string CategoryName { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}
