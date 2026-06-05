using KiemTraGiuaKy.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace KiemTraGiuaKy.Areas.Admin.Controllers
{
    // Câu 2: CRUD Course cho Admin
    // Câu 4: /admin/** -> chỉ ADMIN truy cập
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CourseController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public CourseController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: /Admin/Course
        public async Task<IActionResult> Index()
        {
            var courses = await _context.Courses
                .Include(c => c.Category)
                .OrderBy(c => c.Name)
                .ToListAsync();
            return View(courses);
        }

        // GET: /Admin/Course/Create
        public async Task<IActionResult> Create()
        {
            await LoadCategories();
            return View();
        }

        // POST: /Admin/Course/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Course course, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    course.Image = await SaveImageAsync(imageFile);
                }

                _context.Courses.Add(course);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Course '{course.Name}' created successfully!";
                return RedirectToAction(nameof(Index));
            }

            await LoadCategories(course.CategoryId);
            return View(course);
        }

        // GET: /Admin/Course/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var course = await _context.Courses.FindAsync(id);
            if (course == null) return NotFound();

            await LoadCategories(course.CategoryId);
            return View(course);
        }

        // POST: /Admin/Course/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Course course, IFormFile? imageFile)
        {
            if (id != course.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Giữ lại ảnh cũ nếu không upload ảnh mới
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        // Xóa ảnh cũ nếu có
                        if (!string.IsNullOrEmpty(course.Image))
                        {
                            DeleteImage(course.Image);
                        }
                        course.Image = await SaveImageAsync(imageFile);
                    }

                    _context.Courses.Update(course);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"Course '{course.Name}' updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseExists(course.Id))
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            await LoadCategories(course.CategoryId);
            return View(course);
        }

        // GET: /Admin/Course/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var course = await _context.Courses
                .Include(c => c.Category)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (course == null) return NotFound();
            return View(course);
        }

        // POST: /Admin/Course/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course != null)
            {
                if (!string.IsNullOrEmpty(course.Image))
                    DeleteImage(course.Image);

                _context.Courses.Remove(course);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Course deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        // ======================== HELPERS ========================

        private async Task LoadCategories(int? selectedId = null)
        {
            var cats = await _context.Categories.OrderBy(c => c.Name).ToListAsync();
            ViewBag.Categories = new SelectList(cats, "Id", "Name", selectedId);
        }

        private bool CourseExists(int id) => _context.Courses.Any(e => e.Id == id);

        private async Task<string> SaveImageAsync(IFormFile image)
        {
            var uploadsDir = Path.Combine(_env.WebRootPath, "images", "courses");
            Directory.CreateDirectory(uploadsDir);

            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
            var filePath = Path.Combine(uploadsDir, uniqueFileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await image.CopyToAsync(stream);

            return "/images/courses/" + uniqueFileName;
        }

        private void DeleteImage(string imagePath)
        {
            try
            {
                var fullPath = Path.Combine(_env.WebRootPath, imagePath.TrimStart('/'));
                if (System.IO.File.Exists(fullPath))
                    System.IO.File.Delete(fullPath);
            }
            catch { /* ignore */ }
        }
    }
}
