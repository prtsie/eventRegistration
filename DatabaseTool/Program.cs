using EventRegistration.Database;
using EventRegistration.Database.Models;
using EventRegistration.Database.Models.Users;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace DatabaseTool;

class Program
{
    static async Task Main(string[] args)
    {
        try
        {
            Console.Write("Введи путь к appsettings.json или оставь пустым, если файл в той же директории >>> ");
            var input = Console.ReadLine();
            var path = string.IsNullOrWhiteSpace(input) ? "appsettings.json" : input;

            var root = JObject.Parse(File.ReadAllText(path));

            var connectionStrings =
                root.DescendantsAndSelf()
                    .OfType<JProperty>()
                    .Where(p => p is { Name: "ConnectionStrings"})
                    .Select(p => p.Value)
                    .FirstOrDefault()?
                    .ToObject<Dictionary<string, string>>();

            if (connectionStrings is null || connectionStrings.Count == 0)
            {
                throw new Exception("Не найдено строк подключения.");
            }

            var keys = connectionStrings.Keys.ToArray();

            for (var i = 0; i < connectionStrings.Count; i++)
            {
                Console.WriteLine($"{i}) {keys[i]}: {connectionStrings[keys[i]]}");
            }

            int selected;
            do
            {
                Console.Write("Выберите строку подключения >>> ");
                if (int.TryParse(Console.ReadLine(), out selected) && selected < connectionStrings.Count && selected >= 0)
                {
                    break;
                }

                Console.WriteLine("Такой строки подключения нет");
            } while (true);

            var connectionString = connectionStrings[keys[selected]];
            var optsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optsBuilder.UseSqlServer(connectionString);

            await using var context = new ApplicationDbContext(optsBuilder.Options);

            while (true)
            {
                var counter = 0;
                Console.WriteLine($"{counter++}) Добавить организатора");
                Console.WriteLine($"{counter++}) Удалить организатора");
                Console.WriteLine($"{counter++}) Список организаторов");
                Console.WriteLine($"{counter++}) Создать базу данных");
                Console.WriteLine($"{counter}) Удалить базу данных");
                Console.WriteLine();
                Console.Write("Выберите действие >>> ");

                if (!int.TryParse(Console.ReadLine(), out selected))
                {
                    Console.WriteLine("Неверный ввод");
                    continue;
                }

                Console.WriteLine();

                switch ((Actions)selected)
                {
                    case Actions.AddHost:
                        await AddHost(context);
                        break;
                    case Actions.RemoveHost:
                        await RemoveHost(context);
                        break;
                    case Actions.ListHosts:
                        await ListHosts(context);
                        break;
                    case Actions.CreateDatabase:
                        await CreateDb(context);
                        break;
                    case Actions.DeleteDatabase:
                        await DeleteDb(context);
                        break;
                    default:
                        Console.WriteLine("Неверный ввод");
                        break;
                }

                Console.WriteLine("Тыкни клавишу, чтобы продолжить");
                Console.ReadKey();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }

        Console.ReadKey();

    }

    private static async Task AddHost(ApplicationDbContext context)
    {
        Console.Write("Введите логин >>> ");
        var login = Console.ReadLine()!;

        if (login.Length > Constraints.MaxLoginLength || string.IsNullOrWhiteSpace(login))
        {
            Console.WriteLine($"Логин не должен быть пустым и должен быть не длиннее {Constraints.MaxLoginLength} символов");
            return;
        }

        if (context.GetEntities<User>().SingleOrDefault(u => u.Login == login) is not null)
        {
            Console.WriteLine("Логин уже используется");
            return;
        }

        Console.Write("Введите пароль >>> ");
        var password = Console.ReadLine()!;

        if (password.Length > Constraints.MaxPasswordLength || string.IsNullOrWhiteSpace(password))
        {
            Console.WriteLine($"Пароль не должен быть пустым и должен быть не длиннее {Constraints.MaxLoginLength} символов");
            return;
        }

        context.AddEntity(new User { Login = login, Password = password, Role = Role.Organizer });

        await context.SaveAsync(CancellationToken.None);
        Console.WriteLine("Успешно!");
    }

    private static async Task<List<string>> ListHosts(ApplicationDbContext context)
    {
        var hosts = await context.GetEntities<User>()
            .Where(u => u.Role == Role.Organizer)
            .Select(u => u.Login)
            .ToListAsync();

        if (hosts.Count == 0)
        {
            Console.WriteLine("Организаторов не найдено");
        }
        else
        {
            for (var i = 0; i < hosts.Count; i++)
            {
                Console.WriteLine($"{i}) {hosts[i]}");
            }
        }

        return hosts;
    }

    private static async Task RemoveHost(ApplicationDbContext context)
    {
        var hosts = await ListHosts(context);

        if (hosts.Count == 0)
        {
            Console.WriteLine("Удалять некого");
            return;
        }

        Console.WriteLine();

        int selected;
        do
        {
            Console.Write("Введите номер организатора >>> ");
            if (int.TryParse(Console.ReadLine(), out selected) && selected < hosts.Count && selected >= 0)
            {
                break;
            }

            Console.WriteLine("Неверный ввод");
        } while (true);

        if (AskConfirmation($"Удалить {hosts[selected]}?"))
        {
            var user = await context.GetEntities<User>().SingleOrDefaultAsync(u => u.Login == hosts[selected]);

            if (user is null)
            {
                Console.WriteLine("Организатор не найден");
                return;
            }

            context.Remove(user);
            await context.SaveChangesAsync();

            Console.WriteLine("Успешно!");
        }
    }

    private static async Task CreateDb(ApplicationDbContext context)
    {
        if (AskConfirmation("Это удалит предыдущую базу, если она есть. Продолжить?"))
        {
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();

            Console.WriteLine("Успешно!");
        }
    }

    private static async Task DeleteDb(ApplicationDbContext context)
    {
        if (AskConfirmation("Это удалит базу, если она есть. Продолжить?"))
        {
            await context.Database.EnsureDeletedAsync();

            Console.WriteLine("Успешно!");
        }
    }

    private static bool AskConfirmation(string prompt)
    {
        Console.Write($"{prompt} (y/n) >>> ");
        return Console.ReadLine()!.ToLower() == "y";
    }

    enum Actions
    {
        AddHost,
        RemoveHost,
        ListHosts,
        CreateDatabase,
        DeleteDatabase
    }
}
