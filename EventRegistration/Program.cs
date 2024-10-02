using EventRegistration.Services.DateTimeProvider;

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
