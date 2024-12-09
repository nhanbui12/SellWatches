using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebBanDongHo.Data;
using WebBanDongHo.Helper;
using WebBanDongHo.Models;

var builder = WebApplication.CreateBuilder(args);

//Vnpay
builder.Services.AddScoped<IVnPayService, VnPayService>();

// Add services to the container. DbContext with SQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

//role, user identity
builder.Services.AddDefaultIdentity<ApplicationUsers>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>() 
    .AddDefaultTokenProviders()
    .AddEntityFrameworkStores<ApplicationDbContext>();

//user (role) scoped seed
builder.Services.AddScoped<UserManager<ApplicationUsers>>();
builder.Services.AddControllersWithViews();

builder.Services.AddRazorPages();

//password of user
builder.Services.Configure<IdentityOptions>(options =>
{
    // Change default Password settings.
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
});


//Cart
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    //options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    // use https://learn.microsoft.com/en-us/aspnet/core/fundamentals/app-state?view=aspnetcore-8.0
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// cấu hình middleware pipeline
app.UseHttpsRedirection();
app.UseStaticFiles();


app.UseRouting();

// Kích hoạt middleware Authentication và Authorrization
app.UseAuthentication();
app.UseAuthorization();

//cart
app.UseSession();

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}");


app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
      name: "areas",
      pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}"
    );
});
app.MapRazorPages();

//scope role are Admin and Customer
using (var scope = app.Services.CreateScope())//tạo phạm vi dịch vụ mới, nó sẽ tự giải phóng khi không cần đến
{
    var services = scope.ServiceProvider;
    //lưu trữ dịch vụ cần thiết từ ServiceProvider cung cấp khả năng truy cập các dịch vụ đã được đăng ký trong Dependency Injection (DI)

    //try
    //{
        var context = services.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate();// auto with database apply

        // Seed data if necessary
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUsers>>();

        //nếu hệ thống chưa có vai trò
        if (!roleManager.Roles.Any())
        {
            var roles = new[] { "Admin", "Customer" };// add Admin, User
            foreach (var role in roles)
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
        //nếu không có admin tự động tạo 
        if (!userManager.Users.Any(u => u.UserName == "Admin"))
        {
            var adminUser = new ApplicationUsers
            {
                UserName = "Admin",
                Email = "sellwatches@gmail.com",
                EmailConfirmed = false
            };
            //tạo người dùng admin với mật khẩu admin123
            await userManager.CreateAsync(adminUser, "admin123");
            //thêm người dùng vừa tạo vào vai trò
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    //}
    //catch (Exception ex)
    //{
    //    var logger = services.GetRequiredService<ILogger<Program>>();
    //    logger.LogError(ex, "An error occurred while migrating or seeding the database.");
    //}
}
app.Run();
