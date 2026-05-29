using BTVN5.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BTVN5.Controllers
{
    public class SachController : Controller
    {
        private readonly BookStoreContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SachController(BookStoreContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            var dsSach = await _context.Sachs
                .Include(s => s.ChuDe)
                .ToListAsync();

            return View(dsSach);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sach = await _context.Sachs
                .Include(s => s.ChuDe)
                .FirstOrDefaultAsync(s => s.MaSach == id);

            if (sach == null)
            {
                return NotFound();
            }

            return View(sach);
        }

        public IActionResult Create()
        {
            ViewBag.MaChuDe = new SelectList(_context.ChuDes, "MaChuDe", "TenChuDe");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Sach sach, IFormFile? fileAnh)
        {
            if (fileAnh == null || fileAnh.Length == 0)
            {
                ModelState.AddModelError("HinhAnh", "Vui lòng chọn hình ảnh sách");
            }

            if (ModelState.IsValid)
            {
                if (fileAnh != null && fileAnh.Length > 0)
                {
                    string folderPath = Path.Combine(
                        _webHostEnvironment.WebRootPath,
                        "Content",
                        "ImageBooks"
                    );

                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    string fileName = Path.GetFileName(fileAnh.FileName);
                    string filePath = Path.Combine(folderPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await fileAnh.CopyToAsync(stream);
                    }

                    sach.HinhAnh = fileName;
                }

                sach.NgayCapNhat = DateTime.Now;

                _context.Sachs.Add(sach);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewBag.MaChuDe = new SelectList(_context.ChuDes, "MaChuDe", "TenChuDe", sach.MaChuDe);
            return View(sach);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sach = await _context.Sachs.FindAsync(id);

            if (sach == null)
            {
                return NotFound();
            }

            ViewBag.MaChuDe = new SelectList(_context.ChuDes, "MaChuDe", "TenChuDe", sach.MaChuDe);
            return View(sach);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Sach sach, IFormFile? fileAnh)
        {
            if (id != sach.MaSach)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var sachCu = await _context.Sachs.FindAsync(id);

                if (sachCu == null)
                {
                    return NotFound();
                }

                sachCu.TenSach = sach.TenSach;
                sachCu.TacGia = sach.TacGia;
                sachCu.MoTa = sach.MoTa;
                sachCu.Gia = sach.Gia;
                sachCu.MaChuDe = sach.MaChuDe;
                sachCu.NgayCapNhat = DateTime.Now;

                if (fileAnh != null && fileAnh.Length > 0)
                {
                    string folderPath = Path.Combine(
                        _webHostEnvironment.WebRootPath,
                        "Content",
                        "ImageBooks"
                    );

                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    string fileName = Path.GetFileName(fileAnh.FileName);
                    string filePath = Path.Combine(folderPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await fileAnh.CopyToAsync(stream);
                    }

                    sachCu.HinhAnh = fileName;
                }

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewBag.MaChuDe = new SelectList(_context.ChuDes, "MaChuDe", "TenChuDe", sach.MaChuDe);
            return View(sach);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sach = await _context.Sachs
                .Include(s => s.ChuDe)
                .FirstOrDefaultAsync(s => s.MaSach == id);

            if (sach == null)
            {
                return NotFound();
            }

            return View(sach);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sach = await _context.Sachs.FindAsync(id);

            if (sach != null)
            {
                _context.Sachs.Remove(sach);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}