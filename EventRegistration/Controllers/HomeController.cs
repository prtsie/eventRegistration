using EventRegistration.Database;
using EventRegistration.Database.Models.Events;
using EventRegistration.Database.Models.Users;
using EventRegistration.Requests;
using EventRegistration.Services.DateTimeProvider;
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

        return user.Role switch
        {
            Role.Member => RedirectToAction(nameof(Events)),
            Role.Organizer => RedirectToAction(nameof(HostEvents)),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    [HttpGet]
    public IActionResult HostEvents()
    {
        //TODO сервис для получения идентификатора пользователя
        //TODO дописать action
        return View();
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
