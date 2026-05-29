using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BTVN5.Models
{
    [Table("ChuDe")]
    public class ChuDe
    {
        [Key]
        public int MaChuDe { get; set; }

        [Required(ErrorMessage = "Tên chủ đề không được để trống")]
        public string TenChuDe { get; set; } = string.Empty;

        public ICollection<Sach> Sachs { get; set; } = new List<Sach>();
    }
}