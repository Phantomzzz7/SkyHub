namespace SkyHub.Domain.Entities;

public class Airport
{
    public int Id { get; set; }
    public string IcaoCode { get; set; } = string.Empty;
    public string? IataCode { get; set; }
    public string Name { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }

    public List<Flight> DepartingFlights { get; set; } = new List<Flight>();
    public List<Flight> ArrivingFlights { get; set; } = new List<Flight>();
}