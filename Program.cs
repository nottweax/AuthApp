using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

class Program
{
    static void Main(string[] args)
    {
        using (var context = new AppDbContext())
        {
            context.Database.EnsureCreated(); // Создает базу данных, если она не существует

            while (true)
            {
                Console.WriteLine("1. Авторизация");
                Console.WriteLine("2. Регистрация");
                Console.WriteLine("3. Запись данных");
                Console.WriteLine("4. Отбор и сортировка данных");
                Console.WriteLine("5. Просмотр списка зарегистрированных пользователей");
                Console.WriteLine("0. Выход");
                Console.Write("Выберите действие: ");
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AuthorizeUser(context);
                        break;
                    case "2":
                        RegisterUser(context);
                        break;
                    case "3":
                        RecordData(context);
                        break;
                    case "4":
                        FilterAndSortData(context);
                        break;
                    case "5":
                        ViewRegisteredUsers(context);
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Неверный выбор. Попробуйте снова.");
                        break;
                }
            }
        }
    }

    static void RegisterUser(AppDbContext context)
    {
        Console.Write("Введите имя пользователя: ");
        var username = Console.ReadLine();
        Console.Write("Введите пароль: ");
        var password = Console.ReadLine();

        var passwordHash = HashPassword(password);
        var user = new User { Username = username, PasswordHash = passwordHash };
        context.Users.Add(user);
        context.SaveChanges();
        Console.WriteLine("Пользователь зарегистрирован успешно!");
    }

    static void AuthorizeUser(AppDbContext context)
    {
        Console.Write("Введите имя пользователя: ");
        var username = Console.ReadLine();
        Console.Write("Введите пароль: ");
        var password = Console.ReadLine();

        var user = context.Users.SingleOrDefault(u => u.Username == username);
        if (user != null && VerifyPassword(password, user.PasswordHash))
        {
            Console.WriteLine("Авторизация успешна!");
            // Здесь можно добавить логику для работы с авторизованным пользователем
        }
        else
        {
            Console.WriteLine("Ошибка авторизации.");
        }
    }

    static void RecordData(AppDbContext context)
    {
        Console.Write("Введите имя пользователя: ");
        var username = Console.ReadLine();
        var user = context.Users.SingleOrDefault(u => u.Username == username);

        if (user == null)
        {
            Console.WriteLine("Пользователь не найден.");
            return;
        }

        Console.Write("Введите данные для записи: ");
        var data = Console.ReadLine();

        var dataEntry = new DataEntry { Data = data, UserId = user.Id };
        context.DataEntries.Add(dataEntry);
        context.SaveChanges();
        Console.WriteLine("Данные записаны успешно!");
    }

    static void FilterAndSortData(AppDbContext context)
    {
        Console.Write("Введите имя пользователя: ");
        var username = Console.ReadLine();
        var user = context.Users.SingleOrDefault(u => u.Username == username);

        if (user == null)
        {
            Console.WriteLine("Пользователь не найден.");
            return;
        }

        Console.Write("Введите данные для фильтрации: ");
        var filter = Console.ReadLine();

        var filteredData = context.DataEntries
            .Where(d => d.UserId == user.Id && d.Data.Contains(filter))
            .OrderBy(d => d.Data)
            .ToList();

        Console.WriteLine("Отфильтрованные и отсортированные данные:");
        foreach (var entry in filteredData)
        {
            Console.WriteLine(entry.Data);
        }
    }

    static void ViewRegisteredUsers(AppDbContext context)
    {
        var users = context.Users.ToList();
        Console.WriteLine("Список зарегистрированных пользователей:");
        foreach (var user in users)
        {
            Console.WriteLine(user.Username);
        }
    }

    static string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }

    static bool VerifyPassword(string password, string storedHash)
    {
        var hash = HashPassword(password);
        return hash == storedHash;
    }
}