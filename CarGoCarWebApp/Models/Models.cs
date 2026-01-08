namespace CarGoCarWebApp.Models;

public class UserDto
{
    public int Id { get; set; }
    public string Email { get; set; } = "";
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string? PhoneNumber { get; set; }
    public string Role { get; set; } = "";
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class RideDto
{
    public int Id { get; set; }
    public string FromLocation { get; set; } = "";
    public string ToLocation { get; set; } = "";
    public DateTime DepartureTime { get; set; }
    public int AvailableSeats { get; set; }
    public decimal PricePerSeat { get; set; }
    public string? Description { get; set; }
    public string? Status { get; set; }
    public string? DriverName { get; set; }
    public string? CarInfo { get; set; }
}

public class RideDetailDto : RideDto
{
    public int TotalSeats { get; set; }
    public bool AllowsStops { get; set; }
    public DriverDto? Driver { get; set; }
    public CarDto? Car { get; set; }
    public List<StopDto> Stops { get; set; } = new();
}

public class DriverDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string? PhoneNumber { get; set; }
}

public class CarDto
{
    public int Id { get; set; }
    public string Info { get; set; } = "";
    public string? Color { get; set; }
    public string? LicensePlate { get; set; }
}

public class StopDto
{
    public int Id { get; set; }
    public string Location { get; set; } = "";
    public DateTime ScheduledArrivalTime { get; set; }
    public int StopOrder { get; set; }
    public bool IsOptional { get; set; }
}

public class ReservationDto
{
    public int Id { get; set; }
    public int RideId { get; set; }
    public int NumberOfSeats { get; set; }
    public decimal TotalPrice { get; set; }
    public string Status { get; set; } = "";
    public string? RideInfo { get; set; }
    public DateTime DepartureTime { get; set; }
}

public class CarListDto
{
    public int Id { get; set; }
    public int DriverId { get; set; }
    public string Make { get; set; } = "";
    public string Model { get; set; } = "";
    public int Year { get; set; }
    public string LicensePlate { get; set; } = "";
    public int TotalSeats { get; set; }
    public string Color { get; set; } = "";
    public bool IsActive { get; set; }
}

public class StatsDto
{
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int Drivers { get; set; }
    public int Passengers { get; set; }
    public int TotalRides { get; set; }
    public int ActiveRides { get; set; }
    public int CompletedRides { get; set; }
    public int TotalReservations { get; set; }
    public int ConfirmedReservations { get; set; }
    public int TotalCars { get; set; }
}

