using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using KiemTraGiuaKy.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure Entity Framework Core with SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure ASP.NET Core Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Configure cookie paths for Identity
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.LogoutPath = "/Account/Logout";
});

// Configure Google Authentication (Câu 9)
var googleClientId = builder.Configuration["Authentication:Google:ClientId"];
var googleClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];

if (!string.IsNullOrEmpty(googleClientId) && googleClientId != "YOUR_GOOGLE_CLIENT_ID")
{
    builder.Services.AddAuthentication()
        .AddGoogle(options =>
        {
            options.ClientId = googleClientId!;
            options.ClientSecret = googleClientSecret!;
        });
}

var app = builder.Build();

// Seed Database: Roles and default Admin user
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var context = services.GetRequiredService<ApplicationDbContext>();

        // Create roles: Admin, Student
        string[] roleNames = { "Admin", "Student" };
        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        // Create default Admin user
        var adminEmail = "admin@courseapp.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            var admin = new ApplicationUser
            {
                UserName = "admin",
                Email = adminEmail,
                FullName = "System Administrator",
                EmailConfirmed = true
            };
            var createAdmin = await userManager.CreateAsync(admin, "Admin@123");
            if (createAdmin.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, "Admin");
            }
        }

        // Create default Student user
        var studentEmail = "student@courseapp.com";
        var studentUser = await userManager.FindByEmailAsync(studentEmail);
        if (studentUser == null)
        {
            var student = new ApplicationUser
            {
                UserName = "student",
                Email = studentEmail,
                FullName = "Demo Student",
                EmailConfirmed = true
            };
            var createStudent = await userManager.CreateAsync(student, "Student@123");
            if (createStudent.Succeeded)
            {
                await userManager.AddToRoleAsync(student, "Student");
            }
        }

        // Seed Categories
        if (!context.Categories.Any())
        {
            var categories = new List<Category>
            {
                new Category { Name = "Computer Science" },
                new Category { Name = "Mathematics" },
                new Category { Name = "Physics" },
                new Category { Name = "Business" },
                new Category { Name = "Language" }
            };
            context.Categories.AddRange(categories);
            await context.SaveChangesAsync();
        }

        // Seed Courses
        if (!context.Courses.Any())
        {
            var cs = context.Categories.First(c => c.Name == "Computer Science");
            var math = context.Categories.First(c => c.Name == "Mathematics");
            var physics = context.Categories.First(c => c.Name == "Physics");
            var business = context.Categories.First(c => c.Name == "Business");
            var lang = context.Categories.First(c => c.Name == "Language");

            var courses = new List<Course>
            {
                new Course { Name = "Introduction to Programming", Credits = 3, Lecturer = "Dr. Nguyen Van An", CategoryId = cs.Id, Image = "/images/courses/programming.jpg" },
                new Course { Name = "Data Structures & Algorithms", Credits = 4, Lecturer = "Prof. Tran Thi Bich", CategoryId = cs.Id, Image = "/images/courses/dsa.jpg" },
                new Course { Name = "Web Development", Credits = 3, Lecturer = "Dr. Le Minh Duc", CategoryId = cs.Id, Image = "/images/courses/web.jpg" },
                new Course { Name = "Database Systems", Credits = 3, Lecturer = "Dr. Pham Thi Huong", CategoryId = cs.Id, Image = "/images/courses/database.jpg" },
                new Course { Name = "Calculus I", Credits = 4, Lecturer = "Prof. Hoang Van Khai", CategoryId = math.Id, Image = "/images/courses/calculus.jpg" },
                new Course { Name = "Linear Algebra", Credits = 3, Lecturer = "Dr. Vo Thi Lan", CategoryId = math.Id, Image = "/images/courses/algebra.jpg" },
                new Course { Name = "General Physics", Credits = 3, Lecturer = "Prof. Nguyen Duc Manh", CategoryId = physics.Id, Image = "/images/courses/physics.jpg" },
                new Course { Name = "Business Management", Credits = 3, Lecturer = "Dr. Tran Van Nam", CategoryId = business.Id, Image = "/images/courses/business.jpg" },
                new Course { Name = "English for IT", Credits = 2, Lecturer = "Ms. Pham Thu Oanh", CategoryId = lang.Id, Image = "/images/courses/english.jpg" },
                new Course { Name = "Software Engineering", Credits = 4, Lecturer = "Prof. Le Quoc Phuong", CategoryId = cs.Id, Image = "/images/courses/software.jpg" },
                new Course { Name = "Machine Learning", Credits = 4, Lecturer = "Dr. Hoang Thi Quynh", CategoryId = cs.Id, Image = "/images/courses/ml.jpg" },
                new Course { Name = "Statistics", Credits = 3, Lecturer = "Prof. Vo Van Rong", CategoryId = math.Id, Image = "/images/courses/statistics.jpg" },
            };
            context.Courses.AddRange(courses);
            await context.SaveChangesAsync();
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
