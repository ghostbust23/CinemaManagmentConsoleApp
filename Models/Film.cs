namespace Cinema.Models
{
    public class Film
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Genre { get; set; }
        public string? Director { get; set; }
        public int Duration { get; set; }
        public int ReleaseYear { get; set; }
        public string? AgeRestriction { get; set; }
        public string? Description { get; set; }

        public List<Showtime> Showtimes { get; set; } = new();
    }
}