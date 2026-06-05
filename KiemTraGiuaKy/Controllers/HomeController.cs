using KiemTraGiuaKy.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KiemTraGiuaKy.Controllers
{
    // Câu 1: Trang Home hiển thị danh sách học phần + phân trang (Câu 8: Tìm kiếm)
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const int PageSize = 5; // Câu 1: mỗi trang 5 học phần

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Home/Index hoặc /
        // Câu 1 + Câu 8: Hiển thị danh sách học phần với tìm kiếm và phân trang
        public async Task<IActionResult> Index(string? searchString, int page = 1)
        {
            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentPage"] = page;

            // Câu 8: Tìm kiếm theo tên học phần
            var coursesQuery = _context.Courses
                .Include(c => c.Category)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                coursesQuery = coursesQuery.Where(c =>
                    c.Name.Contains(searchString) ||
                    c.Lecturer.Contains(searchString));
            }

            // Đếm tổng số học phần
            var totalCourses = await coursesQuery.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCourses / (double)PageSize);

            ViewData["TotalPages"] = totalPages;
            ViewData["TotalCourses"] = totalCourses;

            // Câu 1: Phân trang - mỗi trang 5 học phần
            var courses = await coursesQuery
                .OrderBy(c => c.Name)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            // Lấy danh sách enrollments của người dùng hiện tại (nếu đã login)
            if (User.Identity?.IsAuthenticated == true)
            {
                var userId = _context.Users
                    .Where(u => u.UserName == User.Identity.Name)
                    .Select(u => u.Id)
                    .FirstOrDefault();

                var enrolledCourseIds = await _context.Enrollments
                    .Where(e => e.UserId == userId)
                    .Select(e => e.CourseId)
                    .ToListAsync();

                ViewData["EnrolledCourseIds"] = enrolledCourseIds;
                ViewData["UserId"] = userId;
            }

            return View(courses);
        }

        // GET: /Home/Error
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}
