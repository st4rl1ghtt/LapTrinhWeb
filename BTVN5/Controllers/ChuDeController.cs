using BTVN5.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BTVN5.Controllers
{
    public class ChuDeController : Controller
    {
        private readonly BookStoreContext _context;

        public ChuDeController(BookStoreContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var dsChuDe = await _context.ChuDes
                .Select(cd => new ChuDeViewModel
                {
                    MaChuDe = cd.MaChuDe,
                    TenChuDe = cd.TenChuDe,
                    SoLuongSach = cd.Sachs.Count()
                })
                .ToListAsync();

            return View(dsChuDe);
        }
    }
}