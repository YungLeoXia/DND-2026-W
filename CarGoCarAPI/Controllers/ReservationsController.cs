using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarGoCarAPI.Data;
using CarGoCarAPI.Models;

namespace CarGoCarAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReservationsController : ControllerBase
{
    private readonly AppDbContext _db;

    public ReservationsController(AppDbContext db) => _db = db;

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetReservations([FromQuery] int? passengerId)
    {
        var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var isAdmin = User.IsInRole("Admin");

        var query = _db.Reservations
            .Include(r => r.Ride)
            .AsQueryable();

        if (passengerId.HasValue)
        {
            if (passengerId.Value != currentUserId && !isAdmin)
                return Forbid();
            query = query.Where(r => r.PassengerId == passengerId.Value);
        }
        else if (!isAdmin)
        {
            query = query.Where(r => r.PassengerId == currentUserId);
        }

        var reservations = await query
            .Select(r => new
            {
                r.Id,
                r.RideId,
                r.NumberOfSeats,
                r.TotalPrice,
                r.Status,
                RideInfo = $"{r.Ride.FromLocation} → {r.Ride.ToLocation}",
                r.Ride.DepartureTime
            })
            .ToListAsync();

        return Ok(reservations);
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetReservation(int id)
    {
        var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        
        var reservation = await _db.Reservations
            .Include(r => r.Ride)
                .ThenInclude(ride => ride.Driver)
            .Include(r => r.Passenger)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (reservation == null)
            return NotFound(new { error = "Reservation not found" });

        if (reservation.PassengerId != currentUserId && 
            reservation.Ride.DriverId != currentUserId && 
            !User.IsInRole("Admin"))
            return Forbid();

        return Ok(new
        {
            reservation.Id,
            reservation.RideId,
            reservation.NumberOfSeats,
            reservation.PickupLocation,
            reservation.DropoffLocation,
            reservation.TotalPrice,
            reservation.Status,
            reservation.IsDriverConfirmed,
            reservation.BookingTime,
            Passenger = new
            {
                reservation.Passenger.Id,
                Name = $"{reservation.Passenger.FirstName} {reservation.Passenger.LastName}"
            },
            Ride = new
            {
                reservation.Ride.Id,
                reservation.Ride.FromLocation,
                reservation.Ride.ToLocation,
                reservation.Ride.DepartureTime,
                DriverName = $"{reservation.Ride.Driver.FirstName} {reservation.Ride.Driver.LastName}"
            }
        });
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateReservation([FromBody] CreateReservationRequest request)
    {
        var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var passengerId = request.PassengerId > 0 ? request.PassengerId : currentUserId;

        if (passengerId != currentUserId && !User.IsInRole("Admin"))
            return Forbid();

        var ride = await _db.Rides.FindAsync(request.RideId);
        
        if (ride == null)
            return NotFound(new { error = "Ride not found" });

        if (ride.AvailableSeats < request.NumberOfSeats)
            return BadRequest(new { error = "Not enough seats available" });

        if (ride.Status != "Scheduled")
            return BadRequest(new { error = "Ride is not available for booking" });

        if (ride.DriverId == passengerId)
            return BadRequest(new { error = "Cannot book your own ride" });

        var reservation = new Reservation
        {
            RideId = request.RideId,
            PassengerId = passengerId,
            NumberOfSeats = request.NumberOfSeats,
            PickupLocation = request.PickupLocation ?? ride.FromLocation,
            DropoffLocation = ride.ToLocation,
            TotalPrice = ride.PricePerSeat * request.NumberOfSeats,
            Status = "Pending",
            IsDriverConfirmed = false,
            BookingTime = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        ride.AvailableSeats -= request.NumberOfSeats;

        _db.Reservations.Add(reservation);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetReservation), new { id = reservation.Id }, new
        {
            reservation.Id,
            reservation.TotalPrice,
            message = "Reservation created"
        });
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> CancelReservation(int id)
    {
        var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        
        var reservation = await _db.Reservations
            .Include(r => r.Ride)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (reservation == null)
            return NotFound(new { error = "Reservation not found" });

        if (reservation.PassengerId != currentUserId && !User.IsInRole("Admin"))
            return Forbid();

        if (reservation.Status == "Cancelled")
            return BadRequest(new { error = "Reservation already cancelled" });

        reservation.Ride.AvailableSeats += reservation.NumberOfSeats;
        reservation.Status = "Cancelled";
        reservation.CancellationTime = DateTime.UtcNow;
        reservation.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return Ok(new { message = "Reservation cancelled" });
    }

    [Authorize(Roles = "Driver,Admin")]
    [HttpGet("ride/{rideId}")]
    public async Task<IActionResult> GetRideReservations(int rideId)
    {
        var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var ride = await _db.Rides.FindAsync(rideId);

        if (ride == null)
            return NotFound(new { error = "Ride not found" });

        if (ride.DriverId != currentUserId && !User.IsInRole("Admin"))
            return Forbid();

        var reservations = await _db.Reservations
            .Where(r => r.RideId == rideId)
            .Include(r => r.Passenger)
            .Select(r => new
            {
                r.Id,
                r.NumberOfSeats,
                r.Status,
                r.IsDriverConfirmed,
                PassengerName = $"{r.Passenger.FirstName} {r.Passenger.LastName}",
                r.Passenger.PhoneNumber
            })
            .ToListAsync();

        return Ok(reservations);
    }

    [Authorize(Roles = "Driver,Admin")]
    [HttpPost("{id}/confirm")]
    public async Task<IActionResult> ConfirmReservation(int id)
    {
        var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        
        var reservation = await _db.Reservations
            .Include(r => r.Ride)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (reservation == null)
            return NotFound(new { error = "Reservation not found" });

        if (reservation.Ride.DriverId != currentUserId && !User.IsInRole("Admin"))
            return Forbid();

        reservation.Status = "Confirmed";
        reservation.IsDriverConfirmed = true;
        reservation.ConfirmationTime = DateTime.UtcNow;
        reservation.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return Ok(new { message = "Reservation confirmed" });
    }
}

public class CreateReservationRequest
{
    public int RideId { get; set; }
    public int PassengerId { get; set; }
    public int NumberOfSeats { get; set; }
    public string? PickupLocation { get; set; }
}
