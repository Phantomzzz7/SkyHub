using Microsoft.EntityFrameworkCore;
using SkyHub.Domain.Entities;

namespace SkyHub.Infrastructure.Data;

public class SkyHubDbContext : DbContext
{
    public SkyHubDbContext(DbContextOptions<SkyHubDbContext> options) : base(options)
    {
    }

    public DbSet<Airport> Airports => Set<Airport>();
    public DbSet<Airline> Airlines => Set<Airline>();
    public DbSet<Aircraft> Aircraft => Set<Aircraft>();
    public DbSet<Flight> Flights => Set<Flight>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Airport>(entity =>
        {
            entity.Property(a => a.IcaoCode).IsRequired().HasMaxLength(10);
            entity.Property(a => a.IataCode).HasMaxLength(3);
            entity.Property(a => a.Name).IsRequired().HasMaxLength(200);
            entity.Property(a => a.City).HasMaxLength(100);
            entity.Property(a => a.Country).HasMaxLength(100);
            entity.HasIndex(a => a.IcaoCode).IsUnique();
        });

        modelBuilder.Entity<Airline>(entity =>
        {
            entity.Property(a => a.Name).IsRequired().HasMaxLength(200);
            entity.Property(a => a.IataCode).HasMaxLength(3);
            entity.Property(a => a.Country).HasMaxLength(100);
        });

        modelBuilder.Entity<Aircraft>(entity =>
        {
            entity.Property(a => a.Registration).IsRequired().HasMaxLength(20);
            entity.Property(a => a.Model).IsRequired().HasMaxLength(100);
            entity.HasIndex(a => a.Registration).IsUnique();

            entity.HasOne(a => a.Airline)
                .WithMany(al => al.Aircraft)
                .HasForeignKey(a => a.AirlineId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Flight>(entity =>
        {
            entity.Property(f => f.FlightNumber).IsRequired().HasMaxLength(10);
            entity.Property(f => f.BasePrice).HasPrecision(10, 2);
            entity.Property(f => f.Status).HasConversion<string>().HasMaxLength(20);

            entity.HasOne(f => f.DepartureAirport)
                .WithMany(a => a.DepartingFlights)
                .HasForeignKey(f => f.DepartureAirportId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(f => f.ArrivalAirport)
                .WithMany(a => a.ArrivingFlights)
                .HasForeignKey(f => f.ArrivalAirportId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(f => f.Aircraft)
                .WithMany(a => a.Flights)
                .HasForeignKey(f => f.AircraftId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}