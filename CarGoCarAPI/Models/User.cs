namespace CarGoCarAPI.Models;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string? PhoneNumber { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public string Role { get; set; } = "Passenger";
    public bool IsEmailVerified { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public ICollection<Car> Cars { get; set; } = new List<Car>();
    public ICollection<Ride> DriverRides { get; set; } = new List<Ride>();
    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
