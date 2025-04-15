using Cinema.Data;
using Cinema.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cinema.Services
{
    public class CinemaService : ICinemaService
    {
        private readonly CinemaDbContext _context;

        public CinemaService(CinemaDbContext context)
        {
            _context = context;
        }

        public async Task<List<Film>> GetFilmsAsync()
        {
            return await _context.Films.ToListAsync();
        }

        public async Task AddFilmAsync(Film film)
        {
            _context.Films.Add(film);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Hall>> GetHallsAsync()
        {
            return await _context.Halls.ToListAsync();
        }

        public async Task AddHallAsync(Hall hall)
        {
            _context.Halls.Add(hall);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Showtime>> GetShowtimesAsync()
        {
            return await _context.Showtimes
                .Include(s => s.Film)
                .Include(s => s.Hall)
                .ToListAsync();
        }

        public async Task<Showtime> AddShowtimeAsync(int filmId, int hallId, DateTime dateTime, decimal ticketPrice)
        {
            var film = await _context.Films.FindAsync(filmId);
            var hall = await _context.Halls.FindAsync(hallId);
            if (film == null || hall == null)
                throw new Exception("Invalid film or hall");

            var showtime = new Showtime
            {
                FilmId = filmId,
                HallId = hallId,
                DateTime = dateTime,
                TicketPrice = ticketPrice,
                Status = "Active"
            };
            _context.Showtimes.Add(showtime);
            await _context.SaveChangesAsync();
            return showtime;
        }

        public async Task CancelShowtimeAsync(int showtimeId)
        {
            var showtime = await _context.Showtimes.FindAsync(showtimeId);
            if (showtime == null)
                throw new Exception("Showtime not found");

            showtime.Status = "Cancelled";
            await _context.SaveChangesAsync();
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }
        

        public async Task<User> AddUserAsync(string name, string email, string userType)
        {
            var existingUser = await GetUserByEmailAsync(email);
            if (existingUser != null)
                throw new Exception("User with this email already exists");

            var user = new User
            {
                Name = name,
                Email = email,
                UserType = userType,
                BonusPoints = 0
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<Ticket> ReserveTicketAsync(int showtimeId, int seatNumber, int? userId)
        {
            var showtime = await _context.Showtimes
                .Include(s => s.Hall)
                .Include(s => s.Tickets)
                .FirstOrDefaultAsync(s => s.Id == showtimeId);

            if (showtime == null || showtime.Status != "Active")
                throw new Exception("Invalid or cancelled showtime");
            if (seatNumber > showtime.Hall.Capacity || seatNumber <= 0)
                throw new Exception("Invalid seat number");
            if (showtime.Tickets.Any(t => t.SeatNumber == seatNumber && t.Status != "Refunded"))
                throw new Exception("Seat is already taken");

            var ticket = new Ticket
            {
                ShowtimeId = showtimeId,
                SeatNumber = seatNumber,
                Price = showtime.TicketPrice,
                Status = "Reserved",
                UserId = userId
            };
            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();
            return ticket;
        }

        public async Task RefundTicketAsync(int ticketId)
        {
            var ticket = await _context.Tickets.FindAsync(ticketId);
            if (ticket == null || ticket.Status == "Refunded")
                throw new Exception("Invalid ticket or already refunded");

            ticket.Status = "Refunded";
            await _context.SaveChangesAsync();
        }

        public async Task<Sale> PurchaseTicketsAsync(int userId, List<int> ticketIds)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                throw new Exception("User not found");

            var tickets = await _context.Tickets
                .Where(t => ticketIds.Contains(t.Id) && t.Status == "Reserved")
                .ToListAsync();

            if (tickets.Count != ticketIds.Count)
                throw new Exception("Some tickets are not available");

            var totalAmount = tickets.Sum(t => t.Price);
            var sale = new Sale
            {
                UserId = userId,
                TicketCount = tickets.Count,
                TotalAmount = totalAmount,
                PurchaseDate = DateTime.Now
            };

            foreach (var ticket in tickets)
                ticket.Status = "Purchased";

            _context.Sales.Add(sale);

            
            user.BonusPoints += (int)(totalAmount * 0.1m);
            await _context.SaveChangesAsync();
            return sale;
        }

        public async Task<List<Discount>> GetDiscountsAsync()
        {
            return await _context.Discounts
                .Where(d => d.StartDate <= DateTime.Now && d.EndDate >= DateTime.Now)
                .ToListAsync();
        }

        public async Task AddDiscountAsync(string description, decimal percentage, DateTime startDate, DateTime endDate)
        {
            var discount = new Discount
            {
                Description = description,
                DiscountPercentage = percentage,
                StartDate = startDate,
                EndDate = endDate
            };
            _context.Discounts.Add(discount);
            await _context.SaveChangesAsync();
        }

        public async Task ApplyDiscountAsync(int saleId, int discountId)
        {
            var sale = await _context.Sales.FindAsync(saleId);
            var discount = await _context.Discounts.FindAsync(discountId);

            if (sale == null || discount == null || DateTime.Now < discount.StartDate || DateTime.Now > discount.EndDate)
                throw new Exception("Invalid sale or discount");

            sale.TotalAmount *= (1 - discount.DiscountPercentage / 100);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateBonusPointsAsync(int userId, int points)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                throw new Exception("User not found");

            user.BonusPoints += points;
            await _context.SaveChangesAsync();
        }

        public async Task<decimal> GetFinancialStatsAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Sales
                .Where(s => s.PurchaseDate >= startDate && s.PurchaseDate <= endDate)
                .SumAsync(s => s.TotalAmount);
        }
    }
}