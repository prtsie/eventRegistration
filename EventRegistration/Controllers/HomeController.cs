using System.Security.Claims;
using EventRegistration.Database;
using EventRegistration.Database.Models.Events;
using EventRegistration.Database.Models.Users;
using EventRegistration.Requests;
using EventRegistration.Services.MailService;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventRegistration.Controllers;

public class HomeController(IDbContext context, IRegistrationCallbackSender callbackSender) : Controller
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

        var claims = new Claim[] { new(ClaimTypes.Name, user.Login), new(ClaimTypes.Role, user.Role.ToString()) };
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
        var eventItem = await context.GetEntities<Event>()
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
        var events = await context.GetEntities<Event>().ToListAsync(cancellationToken);
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

        var user = new User { Login = request.Login, Password = request.Password, Role = Role.Member };

        context.AddEntity(user); // ����� AddEntity ��� ���������� ������������
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
            Host = user,
            HostName = request.HostName,
            Description = request.Description
        };

        context.AddEntity(newEvent); // ����� AddEntity ��� ���������� �����������
        await context.SaveAsync(cancellationToken);

        return RedirectToAction(nameof(HostEvents));
    }

    [HttpGet]
    [Authorize(Program.HostPolicy)]
    public async Task<IActionResult> CancelEvent(Guid id, CancellationToken cancellationToken)
    {
        var ev = await context.GetEntities<Event>().SingleOrDefaultAsync(e => e.Id == id);

        if (ev is null)
        {
            return NotFound();
        }

        ev.IsCanceled = true;
        await context.SaveAsync(cancellationToken);

        return RedirectToAction(nameof(HostEvents));
    }

    [HttpGet]
    [Authorize(Program.MemberPolicy)]
    public IActionResult SubscribeOnEvent(Guid id)
    {
        return View();
    }

    [HttpPost]
    [Authorize(Program.MemberPolicy)]
    public async Task<IActionResult> SubscribeOnEvent(Guid id, SubscribeRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        var ev = await context.GetEntities<Event>().SingleOrDefaultAsync(e => e.Id == id, cancellationToken);

        var userLogin = User.Claims.First(c => c.Type == ClaimTypes.Name).Value;
        var user = await context.GetEntities<User>()
            .SingleOrDefaultAsync(u => u.Login == userLogin, cancellationToken);

        if (ev is null || user is null)
        {
            return NotFound();
        }

        var foundRegistration = await context.GetEntities<Registration>()
            .Include(r => r.User)
            .Include(r => r.Event)
            .SingleOrDefaultAsync(r => r.User.Id == user.Id && r.Event.Id == ev.Id, cancellationToken);

        if (foundRegistration is not null)
        {
            return Conflict();
        }

        var registration = new Registration
        {
            Firstname = request.Firstname,
            Surname = request.Surname,
            Patronymic = request.Patronymic,
            Email = request.Email,
            Event = ev,
            User = user,
            PhoneNum = request.PhoneNum
        };


        try
        {
            await callbackSender.SendCallbackAsync(registration, cancellationToken);

            context.AddEntity(registration);
            await context.SaveAsync(cancellationToken);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.InnerException?.Message ?? e.Message); //TODO обработка ошибок
            return Problem();
        }


        return RedirectToAction(nameof(Events));
    }

    [HttpGet]
    [Authorize(Program.MemberPolicy)]
    public async Task<IActionResult> Unsubscribe(Guid id, CancellationToken cancellationToken)
    {
        var ev = await context.GetEntities<Event>().SingleOrDefaultAsync(e => e.Id == id, cancellationToken);

        var userLogin = User.Claims.First(c => c.Type == ClaimTypes.Name).Value;
        var user = await context.GetEntities<User>()
            .SingleOrDefaultAsync(u => u.Login == userLogin, cancellationToken);

        if (ev is null || user is null)
        {
            return NotFound();
        }

        var foundRegistration = await context.GetEntities<Registration>()
            .Include(r => r.User)
            .Include(r => r.Event)
            .SingleOrDefaultAsync(r => r.User.Id == user.Id && r.Event.Id == ev.Id, cancellationToken);

        if (foundRegistration is null)
        {
            return BadRequest();
        }

        context.Remove(foundRegistration);
        await context.SaveAsync(cancellationToken);

        return RedirectToAction(nameof(Event), new {Id = id});
    }
}
