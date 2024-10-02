using EventRegistration.Services.DateTimeProvider;
using EventRegistration.Database;
using Microsoft.EntityFrameworkCore;

namespace EventRegistration;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.ConfigureServices();

        var app = builder.Build();
        app.ConfigureRequestPipeline();

        app.Run();
    }

    private static void ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllersWithViews();

        builder.Services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
      
        // Добавляем строку подключения к БД
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

        // Регистрируем контекст базы данных
        builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
    }

    private static void ConfigureRequestPipeline(this WebApplication app)
    {
        app.UseStatusCodePages();

        app.UseStaticFiles();
        app.UseRouting();
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Events}/{id?}");
    }
}
