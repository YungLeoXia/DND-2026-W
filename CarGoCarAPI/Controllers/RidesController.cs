using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarGoCarAPI.Data;
using CarGoCarAPI.Models;

namespace CarGoCarAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RidesController : ControllerBase
{
    private readonly AppDbContext _db;

    public RidesController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetRides(
        [FromQuery] string? from,
        [FromQuery] string? to,
        [FromQuery] DateTime? date,
        [FromQuery] int? minSeats)
    {
        var query = _db.Rides
            .Include(r => r.Driver)
            .Include(r => r.Car)
            .Where(r => r.Status == "Scheduled" && r.DepartureTime > DateTime.UtcNow);

        if (!string.IsNullOrEmpty(from))
            query = query.Where(r => r.FromLocation.ToLower().Contains(from.ToLower()));
        
        if (!string.IsNullOrEmpty(to))
            query = query.Where(r => r.ToLocation.ToLower().Contains(to.ToLower()));
        
        if (date.HasValue)
            query = query.Where(r => r.DepartureTime.Date == date.Value.Date);
        
        if (minSeats.HasValue)
            query = query.Where(r => r.AvailableSeats >= minSeats.Value);

        var rides = await query
            .Select(r => new
            {
                r.Id,
                r.FromLocation,
                r.ToLocation,
                r.DepartureTime,
                r.AvailableSeats,
                r.PricePerSeat,
                DriverName = $"{r.Driver.FirstName} {r.Driver.LastName}",
                CarInfo = $"{r.Car.Make} {r.Car.Model}"
            })
            .ToListAsync();

        return Ok(rides);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetRide(int id)
    {
        var ride = await _db.Rides
            .Include(r => r.Driver)
            .Include(r => r.Car)
            .Include(r => r.Stops.OrderBy(s => s.StopOrder))
            .FirstOrDefaultAsync(r => r.Id == id);

        if (ride == null)
            return NotFound(new { error = "Ride not found" });

        return Ok(new
        {
            ride.Id,
            ride.FromLocation,
            ride.ToLocation,
            ride.DepartureTime,
            ride.ArrivalTime,
            ride.TotalSeats,
            ride.AvailableSeats,
            ride.PricePerSeat,
            ride.Description,
            ride.Status,
            ride.AllowsStops,
            Driver = new
            {
                ride.Driver.Id,
                Name = $"{ride.Driver.FirstName} {ride.Driver.LastName}",
                ride.Driver.PhoneNumber
            },
            Car = new
            {
                ride.Car.Id,
                Info = $"{ride.Car.Year} {ride.Car.Make} {ride.Car.Model}",
                ride.Car.Color,
                ride.Car.LicensePlate
            },
            Stops = ride.Stops.Select(s => new
            {
                s.Id,
                s.Location,
                s.ScheduledArrivalTime,
                s.StopOrder,
                s.IsOptional
            })
        });
    }

    [Authorize(Roles = "Driver,Admin")]
    [HttpPost]
    public async Task<IActionResult> CreateRide([FromBody] CreateRideRequest request)
    {
        var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        
        var car = await _db.Cars.FindAsync(request.CarId);
        if (car == null)
            return BadRequest(new { error = "Car not found" });

        if (car.DriverId != currentUserId && !User.IsInRole("Admin"))
            return Forbid();

        var ride = new Ride
        {
            DriverId = car.DriverId,
            CarId = request.CarId,
            FromLocation = request.FromLocation,
            ToLocation = request.ToLocation,
            DepartureTime = request.DepartureTime,
            TotalSeats = request.AvailableSeats,
            AvailableSeats = request.AvailableSeats,
            PricePerSeat = request.Price,
            Description = request.Description,
            Status = "Scheduled",
            AllowsStops = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _db.Rides.Add(ride);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetRide), new { id = ride.Id }, new { ride.Id, message = "Ride created" });
    }

    [Authorize(Roles = "Driver,Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRide(int id, [FromBody] UpdateRideRequest request)
    {
        var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var ride = await _db.Rides.FindAsync(id);
        
        if (ride == null)
            return NotFound(new { error = "Ride not found" });

        if (ride.DriverId != currentUserId && !User.IsInRole("Admin"))
            return Forbid();

        if (request.DepartureTime != default) ride.DepartureTime = request.DepartureTime;
        if (request.AvailableSeats > 0) ride.AvailableSeats = request.AvailableSeats;
        if (request.Price > 0) ride.PricePerSeat = request.Price;
        if (!string.IsNullOrEmpty(request.Description)) ride.Description = request.Description;
        ride.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return Ok(new { message = "Ride updated" });
    }

    [Authorize(Roles = "Driver,Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> CancelRide(int id)
    {
        var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var ride = await _db.Rides.FindAsync(id);
        
        if (ride == null)
            return NotFound(new { error = "Ride not found" });

        if (ride.DriverId != currentUserId && !User.IsInRole("Admin"))
            return Forbid();

        ride.Status = "Cancelled";
        ride.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return Ok(new { message = "Ride cancelled" });
    }

    [Authorize(Roles = "Driver,Admin")]
    [HttpPost("{id}/stops")]
    public async Task<IActionResult> AddStop(int id, [FromBody] AddStopRequest request)
    {
        var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var ride = await _db.Rides.Include(r => r.Stops).FirstOrDefaultAsync(r => r.Id == id);
        
        if (ride == null)
            return NotFound(new { error = "Ride not found" });

        if (ride.DriverId != currentUserId && !User.IsInRole("Admin"))
            return Forbid();

        if (!ride.AllowsStops)
            return BadRequest(new { error = "This ride does not allow stops" });

        var stop = new Stop
        {
            RideId = id,
            Location = request.Location,
            ScheduledArrivalTime = request.ArrivalTime,
            StopOrder = ride.Stops.Count + 1,
            IsOptional = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _db.Stops.Add(stop);
        await _db.SaveChangesAsync();

        return Ok(new { stopId = stop.Id, message = "Stop added" });
    }

    [HttpGet("driver/{driverId}")]
    public async Task<IActionResult> GetDriverRides(int driverId)
    {
        var rides = await _db.Rides
            .Where(r => r.DriverId == driverId)
            .Select(r => new
            {
                r.Id,
                r.FromLocation,
                r.ToLocation,
                r.DepartureTime,
                r.AvailableSeats,
                r.PricePerSeat,
                r.Status
            })
            .ToListAsync();

        return Ok(rides);
    }
}

public class CreateRideRequest
{
    public int CarId { get; set; }
    public string FromLocation { get; set; } = "";
    public string ToLocation { get; set; } = "";
    public DateTime DepartureTime { get; set; }
    public int AvailableSeats { get; set; }
    public decimal Price { get; set; }
    public string Description { get; set; } = "";
}

public class UpdateRideRequest
{
    public DateTime DepartureTime { get; set; }
    public int AvailableSeats { get; set; }
    public decimal Price { get; set; }
    public string? Description { get; set; }
}

public class AddStopRequest
{
    public string Location { get; set; } = "";
    public DateTime ArrivalTime { get; set; }
}
