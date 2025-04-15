namespace Cinema.Models
{
    public class Hall
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public int Capacity { get; set; }
        public string Type { get; set; } = "Regular";

        public List<Showtime> Showtimes { get; set; } = new();
    }
}