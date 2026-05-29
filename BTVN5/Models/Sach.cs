using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BTVN5.Models
{
    [Table("Sach")]
    public class Sach
    {
        [Key]
        public int MaSach { get; set; }

        [Required(ErrorMessage = "Tên sách không được để trống")]
        public string TenSach { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tác giả không được để trống")]
        public string TacGia { get; set; } = string.Empty;

        public string? MoTa { get; set; }

        [Required(ErrorMessage = "Giá sách không được để trống")]
        public decimal Gia { get; set; }

        public string? HinhAnh { get; set; }

        public DateTime? NgayCapNhat { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn chủ đề")]
        public int MaChuDe { get; set; }

        [ForeignKey("MaChuDe")]
        public ChuDe? ChuDe { get; set; }
    }
}