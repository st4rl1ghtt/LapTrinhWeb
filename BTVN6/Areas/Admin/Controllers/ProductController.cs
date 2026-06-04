using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using BTVN6.Models;
using BTVN6.Repositories;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace BTVN6.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;

        // Dependency Injection cho các repository
        public ProductController(IProductRepository productRepository, ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        // Hiển thị danh sách sản phẩm
        public async Task<IActionResult> Index()
        {
            var products = await _productRepository.GetAllAsync();
            return View(products);
        }

        // Hiển thị form thêm sản phẩm mới
        public async Task<IActionResult> Add()
        {
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View();
        }

        // Xử lý dữ liệu khi submit form thêm sản phẩm
        [HttpPost]
        public async Task<IActionResult> Add(Product product, IFormFile? coverImage, List<IFormFile>? otherImages)
        {
            if (ModelState.IsValid)
            {
                // Nếu có upload ảnh đại diện
                if (coverImage != null)
                {
                    // Kiểm tra định dạng và dung lượng ảnh
                    if (!IsValidImage(coverImage))
                    {
                        ModelState.AddModelError("ImageUrl", "Invalid image format or size is too large.");
                        var categories = await _categoryRepository.GetAllAsync();
                        ViewBag.Categories = new SelectList(categories, "Id", "Name");
                        return View(product);
                    }
                    // Lưu ảnh và lưu đường dẫn vào đối tượng product
                    product.ImageUrl = await SaveImage(coverImage);
                }

                // Nếu có upload danh sách các ảnh khác
                if (otherImages != null && otherImages.Any())
                {
                    product.ImageUrls = new List<string>();
                    foreach (var file in otherImages)
                    {
                        if (IsValidImage(file))
                        {
                            product.ImageUrls.Add(await SaveImage(file));
                        }
                    }
                }

                // Thêm sản phẩm vào danh sách (hoặc database)
                await _productRepository.AddAsync(product);
                // Chuyển hướng về trang danh sách sản phẩm
                return RedirectToAction("Index");
            }
            
            // Nếu dữ liệu không hợp lệ, trả về form kèm theo danh sách categories
            var cats = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(cats, "Id", "Name");
            return View(product);
        }

        // Hiển thị thông tin chi tiết của một sản phẩm
        public async Task<IActionResult> Display(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // Hiển thị form cập nhật thông tin sản phẩm
        public async Task<IActionResult> Update(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // Xử lý dữ liệu khi submit form cập nhật sản phẩm
        [HttpPost]
        public async Task<IActionResult> Update(Product product)
        {
            if (ModelState.IsValid)
            {
                await _productRepository.UpdateAsync(product);
                return RedirectToAction("Index");
            }
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // Hiển thị trang xác nhận xóa sản phẩm
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // Xử lý việc xóa sản phẩm sau khi đã xác nhận
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _productRepository.DeleteAsync(id);
            return RedirectToAction("Index");
        }

        // Hàm hỗ trợ kiểm tra định dạng và dung lượng ảnh
        private bool IsValidImage(IFormFile image)
        {
            var validExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(image.FileName).ToLower();
            if (!validExtensions.Contains(extension))
            {
                return false;
            }
            
            // Giới hạn dung lượng ảnh là 2MB
            if (image.Length > 2 * 1024 * 1024) 
            {
                return false;
            }
            return true;
        }

        // Hàm hỗ trợ lưu ảnh vào thư mục wwwroot/images
        private async Task<string> SaveImage(IFormFile image)
        {
            var dir = "wwwroot/images";
            // Tạo thư mục nếu chưa tồn tại
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            var uniqueFileName = System.Guid.NewGuid().ToString() + "_" + image.FileName;
            var savePath = Path.Combine(dir, uniqueFileName);
            using (var fileStream = new FileStream(savePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }
            // Trả về đường dẫn tương đối để hiển thị trên web
            return "/images/" + uniqueFileName;
        }
    }
}
