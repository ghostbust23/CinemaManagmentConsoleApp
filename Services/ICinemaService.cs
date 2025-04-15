using Cinema.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cinema.Services
{
    public interface ICinemaService
    {
        Task<List<Film>> GetFilmsAsync();
        Task AddFilmAsync(Film film);
        Task<List<Hall>> GetHallsAsync();
        Task AddHallAsync(Hall hall);
        Task<List<Showtime>> GetShowtimesAsync();
        Task<Showtime> AddShowtimeAsync(int filmId, int hallId, DateTime dateTime, decimal ticketPrice);
        Task CancelShowtimeAsync(int showtimeId);
        Task<User> GetUserByEmailAsync(string email);
        Task<User> AddUserAsync(string name, string email, string userType);
        Task<Ticket> ReserveTicketAsync(int showtimeId, int seatNumber, int? userId);
        Task RefundTicketAsync(int ticketId);
        Task<Sale> PurchaseTicketsAsync(int userId, List<int> ticketIds);
        Task<List<Discount>> GetDiscountsAsync();
        Task AddDiscountAsync(string description, decimal percentage, DateTime startDate, DateTime endDate);
        Task ApplyDiscountAsync(int saleId, int discountId);
        Task UpdateBonusPointsAsync(int userId, int points);
        Task<decimal> GetFinancialStatsAsync(DateTime startDate, DateTime endDate);
    }
}