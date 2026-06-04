using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BTVN6.Models
{
    public class Product
    {
        // Định danh sản phẩm
        public int Id { get; set; }

        // Tên sản phẩm, bắt buộc nhập và tối đa 100 ký tự
        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;

        // Giá sản phẩm, giới hạn từ 0.01 đến 10000.00
        [Range(0.01, 10000.00)]
        [System.ComponentModel.DataAnnotations.Schema.Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        // Đoạn mô tả sản phẩm
        public string Description { get; set; } = string.Empty;

        // Khóa ngoại liên kết với danh mục (Category)
        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        // Đường dẫn hình ảnh đại diện (cho phép null)
        public string? ImageUrl { get; set; }
        
        // Danh sách đường dẫn các hình ảnh phụ (cho phép null)
        public List<string>? ImageUrls { get; set; }
    }
}
