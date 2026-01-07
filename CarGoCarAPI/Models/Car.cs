namespace CarGoCarAPI.Models
{
    public class Car
    {
        public int Id { get; set; }
        public int DriverId { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public string LicensePlate { get; set; }
        public int TotalSeats { get; set; }
        public string Color { get; set; }
        public string VinNumber { get; set; }
        public string InsuranceProvider { get; set; }
        public DateTime InsuranceExpiryDate { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        public User Driver { get; set; }
        public ICollection<Ride> Rides { get; set; } = new List<Ride>();
    }
}
