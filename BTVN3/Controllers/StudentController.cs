using Microsoft.AspNetCore.Mvc;
using BTVN3.Models;
using System.Collections.Generic;
using System.Linq;

namespace BTVN3.Controllers
{
    public class StudentController : Controller
    {
        private static List<Student> danhSachSinhVien = new List<Student>();

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ShowKQ(Student student)
        {
            if (student != null)
            {
                danhSachSinhVien.Add(student);
            }

            int soLuongCungNganh = danhSachSinhVien.Count(s => s.ChuyenNganh == student.ChuyenNganh);

            ViewBag.MSSV = student.MSSV;
            ViewBag.HoTen = student.HoTen;
            ViewBag.ChuyenNganh = student.ChuyenNganh;
            ViewBag.SoLuong = soLuongCungNganh;

            return View();
        }
    }
}