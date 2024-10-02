using EventRegistration.Database;
using Microsoft.EntityFrameworkCore;

namespace EventRegistration;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Добавляем строку подключения к БД
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

        // Регистрируем контекст базы данных
        builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
        
        // Add services to the container.
        builder.Services.AddControllersWithViews();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
        }

        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}
