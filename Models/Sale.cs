namespace Cinema.Models;

public class Sale
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int TicketCount { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime PurchaseDate { get; set; }

    public User User { get; set; } = null!;
}