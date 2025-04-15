namespace Cinema.Models;

public class Ticket
{
    public int Id { get; set; }
    public int ShowtimeId { get; set; }
    public int SeatNumber { get; set; }
    public decimal Price { get; set; }
    public string Status { get; set; } = "Reserved";
    public int? UserId { get; set; }

    public Showtime Showtime { get; set; } = null!;
    public User? User { get; set; }
}