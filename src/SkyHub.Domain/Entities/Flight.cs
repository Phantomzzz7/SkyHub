using SkyHub.Domain.Enums;

namespace SkyHub.Domain.Entities;

public class Flight
{
    public int Id { get; set; }
    public string FlightNumber { get; set; } = string.Empty;
    public DateTime DepartureTime { get; set; }
    public DateTime ArrivalTime { get; set; }
    public decimal BasePrice { get; set; }
    public FlightStatus Status { get; set; }

    public int DepartureAirportId { get; set; }
    public Airport DepartureAirport { get; set; } = null!;

    public int ArrivalAirportId { get; set; }
    public Airport ArrivalAirport { get; set; } = null!;

    public int AircraftId { get; set; }
    public Aircraft Aircraft { get; set; } = null!;
}