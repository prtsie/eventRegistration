using System.Security.Claims;
using EventRegistration.Database;
using EventRegistration.Database.Models.Events;
using EventRegistration.Database.Models.Users;
using EventRegistration.Requests;
using EventRegistration.Services.DateTimeProvider;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventRegistration.Controllers;

public class HomeController(IDbContext context, IDateTimeProvider dateTimeProvider) : Controller
{
    [HttpGet]
    public IActionResult Authentication()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Authentication(AuthenticationRequest request, CancellationToken cancellationToken)
    {
        var user = await context.GetEntities<User>()
            .SingleOrDefaultAsync(u => u.Login == request.Login && u.Password == request.Password, cancellationToken);

        if (user is null)
        {
            return View(request);
        }

        var claims = new Claim[]
        {
            new(ClaimTypes.Name, user.Login),
            new(ClaimTypes.Role, user.Role.ToString())
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        await HttpContext.SignInAsync(identity.AuthenticationType, new(identity));

        return user.Role switch
        {
            Role.Member => RedirectToAction(nameof(Events)),
            Role.Organizer => RedirectToAction(nameof(HostEvents)),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(Authentication));
    }

    [HttpGet]
    [Authorize(Program.HostPolicy)]
    public async Task<IActionResult> HostEvents(CancellationToken cancellationToken)
    {
        var login = User.Claims.First(c => c.Type is ClaimTypes.Name).Value;
        var user = await context.GetEntities<User>().SingleOrDefaultAsync(u => u.Login == login, cancellationToken);

        if (user is null)
        {
            return Forbid();
        }

        var events = await context.GetEntities<Event>().Where(e => e.Host.Id == user.Id).ToListAsync(cancellationToken);

        return View(events);
    }

    [HttpGet]
    public IActionResult Event(Guid id)
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Events(CancellationToken cancellationToken)
    {
        var events = await context.GetEntities<Event>().Where(e => e.Date < dateTimeProvider.Now()).ToListAsync(cancellationToken);
        return View(events);
    }
}
