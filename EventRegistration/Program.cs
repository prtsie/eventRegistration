using System.Security.Claims;
using EventRegistration.Services.DateTimeProvider;
using EventRegistration.Database;
using EventRegistration.Database.Models.Users;
using EventRegistration.Services.CardGenerator;
using EventRegistration.Services.MailService;
using EventRegistration.Services.QrGenerator;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Infrastructure;

namespace EventRegistration;

public static class Program
{
    public const string MemberPolicy = "Member";
    public const string HostPolicy = "Organizer";

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
        builder.Services.AddScoped<IDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        builder.Services.AddAuth();

        builder.Services.AddSingleton<IQrGenerator, QrGenerator>();

        QuestPDF.Settings.License = LicenseType.Community;
        builder.Services.AddSingleton<ICardGenerator, PdfCardGenerator>();

        var mailOptions = builder.Configuration.GetSection("MailOptions").Get<MailOptions>()!;

        builder.Services.AddMailService(mailOptions);
    }

    private static void AddAuth(this IServiceCollection services)
    {
        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/home/authentication";
                options.AccessDeniedPath = options.LoginPath;
                options.LogoutPath = "/home/logout";
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
            });
        services.AddAuthorizationBuilder()
            .AddPolicy(MemberPolicy,
                config => config.RequireClaim(ClaimTypes.Role, Role.Member.ToString()))
            .AddPolicy(HostPolicy,
                config => config.RequireClaim(ClaimTypes.Role, Role.Organizer.ToString()));
    }

    private static void AddMailService(this IServiceCollection services, MailOptions options)
    {
        MailService.Options = options;

        services.AddScoped<IRegistrationCallbackSender, MailService>();
    }

    private static void ConfigureRequestPipeline(this WebApplication app)
    {
        app.UseStatusCodePages();

        app.UseHttpsRedirection();

        app.UseStaticFiles();
        app.UseRouting();

        app.UseAuth();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Events}/{id?}");
    }

    private static void UseAuth(this WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
    }
}
