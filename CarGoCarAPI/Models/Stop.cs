namespace CarGoCarAPI.Models;

public class Stop
{
    public int Id { get; set; }
    public int RideId { get; set; }
    public string Location { get; set; } = "";
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public DateTime ScheduledArrivalTime { get; set; }
    public DateTime? ActualArrivalTime { get; set; }
    public int StopOrder { get; set; }
    public bool IsOptional { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Ride Ride { get; set; } = null!;
}
