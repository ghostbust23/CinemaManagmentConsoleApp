using Cinema.Data;
using Cinema.Models;
using Cinema.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        var services = ConfigureServices();
        var serviceProvider = services.BuildServiceProvider();
        var cinemaService = serviceProvider.GetService<ICinemaService>();

        await InitializeDatabase(serviceProvider);

        while (true)
        {
            Console.WriteLine("\n-*-*- Cinema Control -*-*-");
            Console.WriteLine("1.Films list");
            Console.WriteLine("2.Add Film");
            Console.WriteLine("3.Show Halls");
            Console.WriteLine("4.Add Hall");
            Console.WriteLine("5.Showtimes list");
            Console.WriteLine("6.Add showtime");
            Console.WriteLine("7.Cancel showtime");
            Console.WriteLine("8.Reserve ticket");
            Console.WriteLine("9.Buy ticket");
            Console.WriteLine("10.Refund ticket");
            Console.WriteLine("11.Show Discounts");
            Console.WriteLine("12.Add Discount");
            Console.WriteLine("13.Apply Discount");
            Console.WriteLine("14.Add user");
            Console.WriteLine("15.Users list");
            Console.WriteLine("16.Show Finance statistics");
            Console.WriteLine("17.Logout");
            Console.Write("Enter option: ");

            var choice = Console.ReadLine();

            try
            {
                switch (choice)
                {
                    case "1":
                        await ListFilms(cinemaService);
                        break;
                    case "2":
                        await AddFilm(cinemaService);
                        break;
                    case "3":
                        await ListHalls(cinemaService);
                        break;
                    case "4":
                        await AddHall(cinemaService);
                        break;
                    case "5":
                        await ListShowtimes(cinemaService);
                        break;
                    case "6":
                        await AddShowtime(cinemaService);
                        break;
                    case "7":
                        await CancelShowtime(cinemaService);
                        break;
                    case "8":
                        await ReserveTicket(cinemaService);
                        break;
                    case "9":
                        await PurchaseTickets(cinemaService);
                        break;
                    case "10":
                        await RefundTicket(cinemaService);
                        break;
                    case "11":
                        await ListDiscounts(cinemaService);
                        break;
                    case "12":
                        await AddDiscount(cinemaService);
                        break;
                    case "13":
                        await ApplyDiscount(cinemaService);
                        break;
                    case "14":
                        await RegisterUser(cinemaService);
                        break;
                    case "15":
                        await ListUsers(cinemaService);
                        break;
                    case "16":
                        await ShowFinancialStats(cinemaService);
                        break;
                    case "17":
                        return;
                    default:
                        Console.WriteLine("Wrong option!");
                        break; 
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error(sad): {ex.Message}");
            }
        }
    }

    static IServiceCollection ConfigureServices()
    {
        var services = new ServiceCollection();
        services.AddDbContext<CinemaDbContext>(options =>
            options.UseSqlServer("Server=.\\SQLEXPRESS;Database=CinemaDb;Trusted_Connection=True;TrustServerCertificate=True;"));
        services.AddScoped<ICinemaService, CinemaService>();
        return services;
    }

    static async Task InitializeDatabase(ServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<CinemaDbContext>();
        await context.Database.MigrateAsync();

       
        if (!context.Films.Any())
        {
            context.Films.Add(new Film { Title = "Test Film", Duration = 120, ReleaseYear = 2023, Genre = "Horror", AgeRestriction = "18", Description = "Testing sql lol"});
            context.Halls.Add(new Hall { Number = 1, Capacity = 60, Type = "Regular" });
            context.Halls.Add(new Hall { Number = 2, Capacity = 15, Type = "VIP" });
            context.Users.Add(new User { Name = "Adios Bobitos", Email = "Adios_Bobitost@gmail.com", UserType = "Client" });
            context.Discounts.Add(new Discount
            {
                Description = "10% discount for new users",
                DiscountPercentage = 10,
                StartDate = DateTime.Now.AddDays(-1),
                EndDate = DateTime.Now.AddDays(10)
            });
            await context.SaveChangesAsync();
        }
    }

    static async Task ListFilms(ICinemaService service)
    {
        var films = await service.GetFilmsAsync();
        foreach (var film in films)
            Console.WriteLine($"{film.Id}. {film.Title} ({film.ReleaseYear}) \n {film.Duration} mins \n Genre: {film.Genre} \n Age restriction: {film.AgeRestriction} \n Description: {film.Description}");
    }
    
    static async Task User(ICinemaService service)
    {
        var films = await service.GetFilmsAsync();
        foreach (var film in films)
            Console.WriteLine($"{film.Id}. {film.Title} ({film.ReleaseYear}) \n {film.Duration} mins \n Genre: {film.Genre} \n Age restriction: {film.AgeRestriction} \n Description: {film.Description}");
    }

    static async Task AddFilm(ICinemaService service)
    {
        Console.Write("Film name: ");
        string title = Console.ReadLine();
        Console.Write("Genre: ");
        string genre = Console.ReadLine();
        Console.Write("Producer: ");
        string director = Console.ReadLine();
        Console.Write("Duration(mins): ");
        int duration = int.Parse(Console.ReadLine());
        Console.Write("Release year: ");
        int releaseYear = int.Parse(Console.ReadLine());
        Console.Write("Age restriction: ");
        string ageRestriction = Console.ReadLine();
        Console.Write("Description: ");
        string description = Console.ReadLine();

        var film = new Film
        {
            Title = title,
            Genre = genre,
            Director = director,
            Duration = duration,
            ReleaseYear = releaseYear,
            AgeRestriction = ageRestriction,
            Description = description
        };
        await service.AddFilmAsync(film);
        Console.WriteLine("Film added! 5_5");
    }

    static async Task ListHalls(ICinemaService service)
    {
        var halls = await service.GetHallsAsync();
        foreach (var hall in halls)
            Console.WriteLine($"{hall.Id}. Hall {hall.Number}, Capacity: {hall.Capacity}, Type: {hall.Type}");
    }

    static async Task AddHall(ICinemaService service)
    {
        Console.Write("Hall name: ");
        int number = int.Parse(Console.ReadLine());
        Console.Write("Capacity: ");
        int capacity = int.Parse(Console.ReadLine());
        Console.Write("Type (Regular/VIP): ");
        string type = Console.ReadLine();

        var hall = new Hall { Number = number, Capacity = capacity, Type = type };
        await service.AddHallAsync(hall);
        Console.WriteLine("Hall added! 8_8!");
    }

    static async Task ListShowtimes(ICinemaService service)
    {
        var showtimes = await service.GetShowtimesAsync();
        foreach (var showtime in showtimes)
            Console.WriteLine($"{showtime.Id}. {showtime.Film.Title} in hall {showtime.Hall.Number} - {showtime.DateTime} ({showtime.TicketPrice} uah), Status: {showtime.Status}");
    }

    static async Task AddShowtime(ICinemaService service)
    {
        Console.Write("Film id: ");
        int filmId = int.Parse(Console.ReadLine());
        Console.Write("Hall id: ");
        int hallId = int.Parse(Console.ReadLine());
        Console.Write("Date: ");
        DateTime dateTime = DateTime.Parse(Console.ReadLine());
        Console.Write("Ticket price: ");
        decimal ticketPrice = decimal.Parse(Console.ReadLine());

        var showtime = await service.AddShowtimeAsync(filmId, hallId, dateTime, ticketPrice);
        Console.WriteLine($"Showtime {showtime.Id} added! 0_0");
    }

    static async Task CancelShowtime(ICinemaService service)
    {
        Console.Write("Showtime id: ");
        int showtimeId = int.Parse(Console.ReadLine());
        await service.CancelShowtimeAsync(showtimeId);
        Console.WriteLine("Showtime cancelled!");
    }

    static async Task RegisterUser(ICinemaService service)
    {
        Console.Write("Username: ");
        string name = Console.ReadLine();
        Console.Write("Email: ");
        string email = Console.ReadLine();
        Console.Write("Type (Client/Admin): ");
        string userType = Console.ReadLine();

        var user = await service.AddUserAsync(name, email, userType);
        Console.WriteLine($"User {user.Id} registred!");
    }
    
    static async Task ListUsers(ICinemaService service)
    {
        
        var users = await service.GetUsersAsync();
        if (users == null || !users.Any())
        {
            Console.WriteLine("No users found :(.");
            return;
        }

        Console.WriteLine("\n* User list *");
        foreach (var user in users)
        {
            Console.WriteLine($"ID: {user.Id}, Name: {user.Name} , Email: {user.Email}, Type: {user.UserType}");
        }
    }

    static async Task ReserveTicket(ICinemaService service)
    {
        Console.Write("Showtime id: ");
        int showtimeId = int.Parse(Console.ReadLine());
        Console.Write("Seat number: ");
        int seatNumber = int.Parse(Console.ReadLine());
        Console.Write("User Email: ");
        string email = Console.ReadLine();

        int? userId = null;
        if (!string.IsNullOrEmpty(email))
        {
            var user = await service.GetUserByEmailAsync(email);
            if (user == null) throw new Exception("User not found! X_x");
            userId = user.Id;
        }

        var ticket = await service.ReserveTicketAsync(showtimeId, seatNumber, userId);
        Console.WriteLine($"Ticket {ticket.Id} reserved!");
    }

    static async Task PurchaseTickets(ICinemaService service)
    {
        Console.Write("User email: ");
        string email = Console.ReadLine();
        var user = await service.GetUserByEmailAsync(email);
        if (user == null)
            throw new Exception("User not found! X_x");

        Console.Write("Ticket id: ");
        var ticketIds = Console.ReadLine().Split(',').Select(int.Parse).ToList();

        var sale = await service.PurchaseTicketsAsync(user.Id, ticketIds);
        Console.WriteLine($"Sale #{sale.Id} done!\n  Ammount: {sale.TotalAmount} uah, Bonuses: {user.BonusPoints}");
    }

    static async Task RefundTicket(ICinemaService service)
    {
        Console.Write("Ticket id: ");
        int ticketId = int.Parse(Console.ReadLine());
        await service.RefundTicketAsync(ticketId);
        Console.WriteLine("Ticket refunded! :(SAD");
    }

    static async Task ListDiscounts(ICinemaService service)
    {
        var discounts = await service.GetDiscountsAsync();
        foreach (var discount in discounts)
            Console.WriteLine($"{discount.Id}. {discount.Description} ({discount.DiscountPercentage}%) from {discount.StartDate} to {discount.EndDate}");
    }

    static async Task AddDiscount(ICinemaService service)
    {
        Console.Write("Discount description: ");
        string description = Console.ReadLine();
        Console.Write("Discount percentage: ");
        decimal percentage = decimal.Parse(Console.ReadLine());
        Console.Write("Start date: ");
        DateTime startDate = DateTime.Parse(Console.ReadLine());
        Console.Write("End date: ");
        DateTime endDate = DateTime.Parse(Console.ReadLine());

        await service.AddDiscountAsync(description, percentage, startDate, endDate);
        Console.WriteLine("Discount added! 0_0!");
    }

    static async Task ApplyDiscount(ICinemaService service)
    {
        Console.Write("Sale id: ");
        int saleId = int.Parse(Console.ReadLine());
        Console.Write("Discount id: ");
        int discountId = int.Parse(Console.ReadLine());

        await service.ApplyDiscountAsync(saleId, discountId);
        Console.WriteLine("Discount applied! ^_^!");
    }

    static async Task ShowFinancialStats(ICinemaService service)
    {
        Console.Write("Start date: ");
        DateTime startDate = DateTime.Parse(Console.ReadLine());
        Console.Write("End date: ");
        DateTime endDate = DateTime.Parse(Console.ReadLine());

        var totalRevenue = await service.GetFinancialStatsAsync(startDate, endDate);
        Console.WriteLine($"Total revenue : {totalRevenue} uah *_*");
    }
}