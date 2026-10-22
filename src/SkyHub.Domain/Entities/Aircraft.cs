namespace SkyHub.Domain.Entities;

public class Aircraft
{
    public int Id { get; set; }
    public string Registration { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Rows { get; set; }
    public int SeatsPerRow { get; set; }

    public int AirlineId { get; set; }
    public Airline Airline { get; set; } = null!;

    public List<Flight> Flights { get; set; } = new List<Flight>();
}