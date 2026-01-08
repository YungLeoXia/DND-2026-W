using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarGoCarAPI.Data;
using CarGoCarAPI.Models;

namespace CarGoCarAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CarsController : ControllerBase
{
    private readonly AppDbContext _db;

    public CarsController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetCars([FromQuery] int? driverId)
    {
        var query = _db.Cars.AsQueryable();
        
        if (driverId.HasValue)
            query = query.Where(c => c.DriverId == driverId.Value);

        var cars = await query
            .Select(c => new
            {
                c.Id,
                c.DriverId,
                c.Make,
                c.Model,
                c.Year,
                c.LicensePlate,
                c.TotalSeats,
                c.Color,
                c.IsActive
            })
            .ToListAsync();

        return Ok(cars);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCar(int id)
    {
        var car = await _db.Cars
            .Include(c => c.Driver)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (car == null)
            return NotFound(new { error = "Car not found" });

        return Ok(new
        {
            car.Id,
            car.DriverId,
            DriverName = $"{car.Driver.FirstName} {car.Driver.LastName}",
            car.Make,
            car.Model,
            car.Year,
            car.LicensePlate,
            car.TotalSeats,
            car.Color,
            car.VinNumber,
            car.InsuranceProvider,
            car.InsuranceExpiryDate,
            car.IsActive
        });
    }

    [Authorize(Roles = "Driver,Admin")]
    [HttpPost]
    public async Task<IActionResult> CreateCar([FromBody] CreateCarRequest request)
    {
        var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var driverId = request.DriverId > 0 ? request.DriverId : currentUserId;

        var driver = await _db.Users.FindAsync(driverId);
        if (driver == null)
            return BadRequest(new { error = "Driver not found" });

        var car = new Car
        {
            DriverId = driverId,
            Make = request.Make,
            Model = request.Model,
            Year = request.Year,
            LicensePlate = request.LicensePlate,
            TotalSeats = request.Seats,
            Color = request.Color,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _db.Cars.Add(car);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetCar), new { id = car.Id }, new { car.Id, message = "Car created" });
    }

    [Authorize(Roles = "Driver,Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCar(int id, [FromBody] UpdateCarRequest request)
    {
        var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var car = await _db.Cars.FindAsync(id);
        
        if (car == null)
            return NotFound(new { error = "Car not found" });

        if (car.DriverId != currentUserId && !User.IsInRole("Admin"))
            return Forbid();

        if (!string.IsNullOrEmpty(request.Make)) car.Make = request.Make;
        if (!string.IsNullOrEmpty(request.Model)) car.Model = request.Model;
        if (request.Year > 0) car.Year = request.Year;
        if (!string.IsNullOrEmpty(request.LicensePlate)) car.LicensePlate = request.LicensePlate;
        if (request.Seats > 0) car.TotalSeats = request.Seats;
        if (!string.IsNullOrEmpty(request.Color)) car.Color = request.Color;
        car.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return Ok(new { message = "Car updated" });
    }

    [Authorize(Roles = "Driver,Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCar(int id)
    {
        var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var car = await _db.Cars.FindAsync(id);
        
        if (car == null)
            return NotFound(new { error = "Car not found" });

        if (car.DriverId != currentUserId && !User.IsInRole("Admin"))
            return Forbid();

        _db.Cars.Remove(car);
        await _db.SaveChangesAsync();

        return Ok(new { message = "Car deleted" });
    }
}

public class CreateCarRequest
{
    public int DriverId { get; set; }
    public string Make { get; set; } = "";
    public string Model { get; set; } = "";
    public int Year { get; set; }
    public string LicensePlate { get; set; } = "";
    public int Seats { get; set; }
    public string Color { get; set; } = "";
}

public class UpdateCarRequest
{
    public string? Make { get; set; }
    public string? Model { get; set; }
    public int Year { get; set; }
    public string? LicensePlate { get; set; }
    public int Seats { get; set; }
    public string? Color { get; set; }
}
