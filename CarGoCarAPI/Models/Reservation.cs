namespace CarGoCarAPI.Models;

public class Reservation
{
    public int Id { get; set; }
    public int RideId { get; set; }
    public int PassengerId { get; set; }
    public int NumberOfSeats { get; set; }
    public string? PickupLocation { get; set; }
    public string? DropoffLocation { get; set; }
    public decimal TotalPrice { get; set; }
    public string Status { get; set; } = "Pending";
    public bool IsDriverConfirmed { get; set; }
    public DateTime BookingTime { get; set; }
    public DateTime? ConfirmationTime { get; set; }
    public DateTime? CancellationTime { get; set; }
    public string? CancellationReason { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Ride Ride { get; set; } = null!;
    public User Passenger { get; set; } = null!;
}
