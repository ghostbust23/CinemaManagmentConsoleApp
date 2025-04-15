using Microsoft.EntityFrameworkCore;
using Cinema.Models;

namespace Cinema.Data
{
    public class CinemaDbContext : DbContext
    {
        public CinemaDbContext(DbContextOptions<CinemaDbContext> options) : base(options) { }

        public CinemaDbContext()
        {
        }

        public DbSet<Film> Films { get; set; }
        public DbSet<Hall> Halls { get; set; }
        public DbSet<Showtime> Showtimes { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<Discount> Discounts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Ticket>()
                .HasCheckConstraint("CK_Ticket_SeatNumber", "SeatNumber > 0");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=.\\SQLEXPRESS;Database=CinemaDb;Trusted_Connection=True;TrustServerCertificate=True;");
            }
        }
    }
}