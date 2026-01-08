using Microsoft.EntityFrameworkCore;
using CarGoCarAPI.Models;

namespace CarGoCarAPI.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Car> Cars => Set<Car>();
    public DbSet<Ride> Rides => Set<Ride>();
    public DbSet<Reservation> Reservations => Set<Reservation>();
    public DbSet<Stop> Stops => Set<Stop>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.HasIndex(u => u.Email).IsUnique();
            entity.HasMany(u => u.Cars).WithOne(c => c.Driver).HasForeignKey(c => c.DriverId);
            entity.HasMany(u => u.DriverRides).WithOne(r => r.Driver).HasForeignKey(r => r.DriverId);
            entity.HasMany(u => u.Reservations).WithOne(r => r.Passenger).HasForeignKey(r => r.PassengerId);
        });

        modelBuilder.Entity<Car>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.HasMany(c => c.Rides).WithOne(r => r.Car).HasForeignKey(r => r.CarId);
        });

        modelBuilder.Entity<Ride>(entity =>
        {
            entity.HasKey(r => r.Id);
            entity.HasMany(r => r.Stops).WithOne(s => s.Ride).HasForeignKey(s => s.RideId);
            entity.HasMany(r => r.Reservations).WithOne(r => r.Ride).HasForeignKey(r => r.RideId);
        });

        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.HasKey(r => r.Id);
        });

        modelBuilder.Entity<Stop>(entity =>
        {
            entity.HasKey(s => s.Id);
        });

        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        var now = DateTime.UtcNow;

        var users = new[]
        {
            new User { Id = 1, Email = "admin@cargocar.com", PasswordHash = "admin123", FirstName = "Admin", LastName = "User", Role = "Admin", IsEmailVerified = true, IsActive = true, CreatedAt = now, UpdatedAt = now },
            new User { Id = 2, Email = "john@example.com", PasswordHash = "pass123", FirstName = "John", LastName = "Smith", PhoneNumber = "+1234567890", Role = "Driver", IsEmailVerified = true, IsActive = true, CreatedAt = now, UpdatedAt = now },
            new User { Id = 3, Email = "jane@example.com", PasswordHash = "pass123", FirstName = "Jane", LastName = "Doe", PhoneNumber = "+0987654321", Role = "Driver", IsEmailVerified = true, IsActive = true, CreatedAt = now, UpdatedAt = now },
            new User { Id = 4, Email = "bob@example.com", PasswordHash = "pass123", FirstName = "Bob", LastName = "Wilson", Role = "Passenger", IsEmailVerified = true, IsActive = true, CreatedAt = now, UpdatedAt = now },
            new User { Id = 5, Email = "alice@example.com", PasswordHash = "pass123", FirstName = "Alice", LastName = "Brown", Role = "Passenger", IsEmailVerified = false, IsActive = true, CreatedAt = now, UpdatedAt = now },
        };
        modelBuilder.Entity<User>().HasData(users);

        var cars = new[]
        {
            new Car { Id = 1, DriverId = 2, Make = "Toyota", Model = "Camry", Year = 2022, LicensePlate = "ABC-1234", TotalSeats = 4, Color = "Silver", IsActive = true, CreatedAt = now, UpdatedAt = now },
            new Car { Id = 2, DriverId = 2, Make = "Honda", Model = "CR-V", Year = 2021, LicensePlate = "XYZ-5678", TotalSeats = 5, Color = "Black", IsActive = true, CreatedAt = now, UpdatedAt = now },
            new Car { Id = 3, DriverId = 3, Make = "Ford", Model = "Focus", Year = 2020, LicensePlate = "DEF-9012", TotalSeats = 4, Color = "Blue", IsActive = true, CreatedAt = now, UpdatedAt = now },
        };
        modelBuilder.Entity<Car>().HasData(cars);

        var rides = new[]
        {
            new Ride { Id = 1, DriverId = 2, CarId = 1, FromLocation = "København", ToLocation = "Aarhus", DepartureTime = now.AddDays(1), TotalSeats = 4, AvailableSeats = 3, PricePerSeat = 150.00m, Status = "Scheduled", AllowsStops = true, CreatedAt = now, UpdatedAt = now },
            new Ride { Id = 2, DriverId = 2, CarId = 2, FromLocation = "Aarhus", ToLocation = "Aalborg", DepartureTime = now.AddDays(2), TotalSeats = 5, AvailableSeats = 4, PricePerSeat = 120.00m, Status = "Scheduled", AllowsStops = true, CreatedAt = now, UpdatedAt = now },
            new Ride { Id = 3, DriverId = 3, CarId = 3, FromLocation = "Odense", ToLocation = "København", DepartureTime = now.AddDays(3), TotalSeats = 4, AvailableSeats = 2, PricePerSeat = 130.00m, Status = "Scheduled", AllowsStops = false, CreatedAt = now, UpdatedAt = now },
        };
        modelBuilder.Entity<Ride>().HasData(rides);

        var reservations = new[]
        {
            new Reservation { Id = 1, RideId = 1, PassengerId = 4, NumberOfSeats = 1, PickupLocation = "København", DropoffLocation = "Aarhus", TotalPrice = 150.00m, Status = "Confirmed", IsDriverConfirmed = true, BookingTime = now, CreatedAt = now, UpdatedAt = now },
            new Reservation { Id = 2, RideId = 3, PassengerId = 4, NumberOfSeats = 2, PickupLocation = "Odense", DropoffLocation = "København", TotalPrice = 260.00m, Status = "Pending", IsDriverConfirmed = false, BookingTime = now, CreatedAt = now, UpdatedAt = now },
        };
        modelBuilder.Entity<Reservation>().HasData(reservations);

        var stops = new[]
        {
            new Stop { Id = 1, RideId = 1, Location = "Odense", Latitude = 55.4038m, Longitude = 10.4024m, ScheduledArrivalTime = now.AddDays(1).AddHours(1), StopOrder = 1, IsOptional = true, CreatedAt = now, UpdatedAt = now },
            new Stop { Id = 2, RideId = 2, Location = "Randers", Latitude = 56.4607m, Longitude = 10.0362m, ScheduledArrivalTime = now.AddDays(2).AddHours(1), StopOrder = 1, IsOptional = false, CreatedAt = now, UpdatedAt = now },
        };
        modelBuilder.Entity<Stop>().HasData(stops);
    }
}

