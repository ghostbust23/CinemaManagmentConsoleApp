namespace Cinema.Models;

public class Showtime
{
    public int Id { get; set; }
    public int FilmId { get; set; }
    public int HallId { get; set; }
    public DateTime DateTime { get; set; }
    public decimal TicketPrice { get; set; }
    public string Status { get; set; } = "Active";

    public Film Film { get; set; } = null!;
    public Hall Hall { get; set; } = null!;
    public List<Ticket> Tickets { get; set; } = new();
}