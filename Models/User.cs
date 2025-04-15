namespace Cinema.Models;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string UserType { get; set; } = "Client";
    public int BonusPoints { get; set; }

    public List<Ticket> Tickets { get; set; } = new();
    public List<Sale> Sales { get; set; } = new();
}