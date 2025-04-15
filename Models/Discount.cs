namespace Cinema.Models;

public class Discount
{
    public int Id { get; set; }
    public string Description { get; set; } = null!;
    public decimal DiscountPercentage { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}