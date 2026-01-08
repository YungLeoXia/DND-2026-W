namespace CarGoCarAPI.Models;

public class Ride
{
    public int Id { get; set; }
    public int DriverId { get; set; }
    public int CarId { get; set; }
    public string FromLocation { get; set; } = "";
    public string ToLocation { get; set; } = "";
    public DateTime DepartureTime { get; set; }
    public DateTime? ArrivalTime { get; set; }
    public int TotalSeats { get; set; }
    public int AvailableSeats { get; set; }
    public decimal PricePerSeat { get; set; }
    public string? Description { get; set; }
    public string Status { get; set; } = "Scheduled";
    public bool AllowsStops { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public User Driver { get; set; } = null!;
    public Car Car { get; set; } = null!;
    public ICollection<Stop> Stops { get; set; } = new List<Stop>();
    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
