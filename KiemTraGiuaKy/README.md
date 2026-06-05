# KiemTraGiuaKy - Course Registration Application

Ứng dụng đăng ký học phần xây dựng bằng ASP.NET Core MVC, Entity Framework Core, và ASP.NET Core Identity.

## Hướng dẫn Build & Chạy (Windows / Visual Studio)

### 1. Mở Project
- Mở `KiemTraGiuaKy.slnx` bằng **Visual Studio 2022+**

### 2. Cập nhật Connection String
Mở `appsettings.json`, cập nhật `DefaultConnection` cho phù hợp với SQL Server của bạn:
```json
"DefaultConnection": "Server=.;Database=CourseRegistrationDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
```

### 3. Tạo Database (Migration)
Mở **Package Manager Console** (Tools → NuGet Package Manager → Package Manager Console) và chạy:
```powershell
Add-Migration InitialCreate
Update-Database
```
> Database `CourseRegistrationDB` sẽ được tạo tự động với tất cả các bảng cần thiết.

### 4. Chạy Ứng dụng
- Nhấn **F5** hoặc **Ctrl+F5** để chạy
- Ứng dụng sẽ tự động seed:
  - **Admin account:** `admin@courseapp.com` / `Admin@123`
  - **Student account:** `student@courseapp.com` / `Student@123`
  - 12 khóa học mẫu và 5 category

### 5. Cấu hình Google Login (Câu 9 - Tùy chọn)
Cập nhật `appsettings.json`:
```json
"Authentication": {
    "Google": {
        "ClientId": "YOUR_GOOGLE_CLIENT_ID",
        "ClientSecret": "YOUR_GOOGLE_CLIENT_SECRET"
    }
}
```
> Nếu không cấu hình, Google Login button vẫn hiển thị nhưng không hoạt động. Các chức năng khác vẫn hoạt động bình thường.

---

## Cấu trúc Database

### Bảng từ ASP.NET Core Identity (tự động tạo):
- `AspNetUsers` - Thông tin người dùng (kế thừa `ApplicationUser`)
- `AspNetRoles` - Vai trò: **Admin**, **Student**
- `AspNetUserRoles` - Liên kết user-role

### Bảng tự xây dựng:
| Bảng | Cột |
|------|-----|
| `Categories` | Id, Name |
| `Courses` | Id, Name, Image, Credits, Lecturer, CategoryId (FK) |
| `Enrollments` | Id, UserId (FK → AspNetUsers), CourseId (FK → Courses), EnrollDate |

---

## Các Chức năng Đã Triển khai

| Câu | Điểm | Chức năng | Controller/Action |
|-----|------|-----------|-------------------|
| 1 | 2.5đ | Home - danh sách học phần + phân trang (5/trang) | `HomeController.Index` |
| 2 | 1.5đ | CRUD học phần cho Admin | `Areas/Admin/CourseController` |
| 3 | 1.0đ | Đăng ký tài khoản (role mặc định: Student) | `AccountController.Register` |
| 4 | 0.5đ | Authorization: /admin → Admin, /enroll → Student | `[Authorize(Roles="...")]` |
| 5 | 0.5đ | Đăng nhập → redirect về /home | `AccountController.Login` |
| 6 | 1.0đ | Enroll/Unenroll học phần (chỉ Student) | `EnrollController.Enroll/Unenroll` |
| 7 | 0.5đ | My Courses - xem học phần đã đăng ký | `EnrollController.MyCourses` |
| 8 | 0.5đ | Tìm kiếm học phần theo tên | `HomeController.Index?searchString=...` |
| 9 | 1.0đ | Google External Login | `AccountController.ExternalLogin` |
| 10 | 1.0đ | Admin Dashboard thống kê | `Areas/Admin/DashboardController` |
| **Tổng** | **10đ** | | |

---

## Phân quyền (Authorization)

| Route | Quyền truy cập |
|-------|----------------|
| `/` và `/Home/**` | Tất cả (kể cả chưa login) |
| `/Account/Login` | Chưa đăng nhập |
| `/Account/Register` | Chưa đăng nhập |
| `/Enroll/**` | Chỉ **Student** |
| `/Admin/**` | Chỉ **Admin** |
