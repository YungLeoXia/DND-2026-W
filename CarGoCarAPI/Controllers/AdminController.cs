using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarGoCarAPI.Data;

namespace CarGoCarAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly AppDbContext _db;

    public AdminController(AppDbContext db) => _db = db;

    [HttpGet("users")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _db.Users
            .Select(u => new
            {
                u.Id,
                u.Email,
                Name = $"{u.FirstName} {u.LastName}",
                u.FirstName,
                u.LastName,
                u.Role,
                u.IsActive,
                u.IsEmailVerified,
                u.CreatedAt
            })
            .ToListAsync();

        return Ok(users);
    }

    [HttpPost("users/{id}/disable")]
    public async Task<IActionResult> DisableUser(int id)
    {
        var user = await _db.Users.FindAsync(id);
        
        if (user == null)
            return NotFound(new { error = "User not found" });

        if (user.Role == "Admin")
            return BadRequest(new { error = "Cannot disable admin users" });

        user.IsActive = false;
        user.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return Ok(new { message = "User disabled" });
    }

    [HttpPost("users/{id}/enable")]
    public async Task<IActionResult> EnableUser(int id)
    {
        var user = await _db.Users.FindAsync(id);
        
        if (user == null)
            return NotFound(new { error = "User not found" });

        user.IsActive = true;
        user.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return Ok(new { message = "User enabled" });
    }

    [HttpGet("rides")]
    public async Task<IActionResult> GetAllRides()
    {
        var rides = await _db.Rides
            .Include(r => r.Driver)
            .Select(r => new
            {
                r.Id,
                DriverName = $"{r.Driver.FirstName} {r.Driver.LastName}",
                r.FromLocation,
                r.ToLocation,
                r.DepartureTime,
                r.Status,
                r.AvailableSeats,
                r.CreatedAt
            })
            .ToListAsync();

        return Ok(rides);
    }

    [HttpDelete("rides/{id}")]
    public async Task<IActionResult> RemoveRide(int id)
    {
        var ride = await _db.Rides.FindAsync(id);
        
        if (ride == null)
            return NotFound(new { error = "Ride not found" });

        _db.Rides.Remove(ride);
        await _db.SaveChangesAsync();

        return Ok(new { message = "Ride removed" });
    }

    [HttpGet("statistics")]
    public async Task<IActionResult> GetStatistics()
    {
        var stats = new
        {
            TotalUsers = await _db.Users.CountAsync(),
            ActiveUsers = await _db.Users.CountAsync(u => u.IsActive),
            Drivers = await _db.Users.CountAsync(u => u.Role == "Driver"),
            Passengers = await _db.Users.CountAsync(u => u.Role == "Passenger"),
            TotalRides = await _db.Rides.CountAsync(),
            ActiveRides = await _db.Rides.CountAsync(r => r.Status == "Scheduled"),
            CompletedRides = await _db.Rides.CountAsync(r => r.Status == "Completed"),
            TotalReservations = await _db.Reservations.CountAsync(),
            ConfirmedReservations = await _db.Reservations.CountAsync(r => r.Status == "Confirmed"),
            TotalCars = await _db.Cars.CountAsync()
        };

        return Ok(stats);
    }

    [HttpPost("users/{id}/role")]
    public async Task<IActionResult> UpdateUserRole(int id, [FromBody] UpdateRoleRequest request)
    {
        var user = await _db.Users.FindAsync(id);
        
        if (user == null)
            return NotFound(new { error = "User not found" });

        var validRoles = new[] { "Admin", "Driver", "Passenger" };
        if (!validRoles.Contains(request.Role))
            return BadRequest(new { error = "Invalid role" });

        user.Role = request.Role;
        user.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return Ok(new { message = $"User role updated to {request.Role}" });
    }
}

public class UpdateRoleRequest
{
    public string Role { get; set; } = "";
}
