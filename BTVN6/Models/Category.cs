using System.ComponentModel.DataAnnotations;

namespace BTVN6.Models
{
    public class Category
    {
        // Định danh của danh mục
        public int Id { get; set; }

        // Tên danh mục, bắt buộc nhập và tối đa 50 ký tự
        [Required, StringLength(50)]
        public string Name { get; set; } = string.Empty;

        // Danh sách các sản phẩm thuộc danh mục này
        public List<Product>? Products { get; set; }
    }
}
