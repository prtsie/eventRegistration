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
    public async Task<IActionResult> Authentication(AuthenticationRequest request, [FromQuery] string? returnUrl, CancellationToken cancellationToken)
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

        if (returnUrl is not null)
        {
            return LocalRedirect(returnUrl);
        }
        return user.Role switch
        {
            Role.Member => RedirectToAction(nameof(Events)),
            Role.Organizer => RedirectToAction(nameof(HostEvents)),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    [HttpGet]
    public async Task<IActionResult> Logout(string returnUrl = "/")
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return LocalRedirect(returnUrl);
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
    public async Task<IActionResult> Event(Guid id, CancellationToken cancellationToken)
    {
        var eventItem = await context.GetEntities<Event>().Include(e => e.Host) //TODO убрать, когда в Event появится свойство с организатором
            .SingleOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (eventItem == null)
        {
            return NotFound();
        }

        return View(eventItem);
    }

    [HttpGet]
    public async Task<IActionResult> Events(CancellationToken cancellationToken)
    {
        var events = await context.GetEntities<Event>().Where(e => e.Date < dateTimeProvider.Now()).ToListAsync(cancellationToken);
        return View(events);
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(request);
        }

        var foundUser = await context.GetEntities<User>().SingleOrDefaultAsync(u => u.Login == request.Login, cancellationToken);
        if (foundUser is not null)
        {
            return View(request);
        }

        var user = new User
        {
            Login = request.Login,
            Password = request.Password,
            Role = Role.Member
        };

        context.AddEntity(user);  // ����� AddEntity ��� ���������� ������������
        await context.SaveAsync(cancellationToken);

        return RedirectToAction(nameof(Authentication));
    }

    [HttpGet]
    [Authorize(Program.HostPolicy)] // ���������������� ��� ������������ (������� ������������ �� ����� ���� ��� ������� � ���� ��������)
    public IActionResult CreateEvent()
    {
        return View();
    }

    [HttpPost]
    [Authorize(Program.HostPolicy)]
    public async Task<IActionResult> CreateEvent(CreateEventRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(request);
        }

        var login = User.Claims.First(c => c.Type == ClaimTypes.Name).Value;
        var user = await context.GetEntities<User>().SingleOrDefaultAsync(u => u.Login == login, cancellationToken);

        if (user == null)
        {
            return Forbid();
        }

        var newEvent = new Event
        {
            Name = request.Name,
            Date = request.Date,
            Host = user
        };

        context.AddEntity(newEvent);  // ����� AddEntity ��� ���������� �����������
        await context.SaveAsync(cancellationToken);

        return RedirectToAction(nameof(HostEvents));
    }

}
