using Microsoft.AspNetCore.Mvc;

namespace CarGoCarAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CarsController : ControllerBase
    {
        /// <summary>
        /// Get all cars for the current driver
        /// </summary>
        [HttpGet]
        public IActionResult GetMyCars()
        {
            // TODO: Implement getting user's cars
            return Ok(new[] { new { id = 1, make = "Toyota", model = "Camry", seats = 5 } });
        }

        /// <summary>
        /// Get a specific car by ID
        /// </summary>
        [HttpGet("{id}")]
        public IActionResult GetCar(int id)
        {
            // TODO: Implement car retrieval
            return Ok(new { id, make = "Toyota", model = "Camry", seats = 5, licensePlate = "ABC123" });
        }

        /// <summary>
        /// Register a new car
        /// </summary>
        [HttpPost]
        public IActionResult CreateCar([FromBody] CreateCarRequest request)
        {
            // TODO: Implement car creation
            return CreatedAtAction(nameof(GetCar), new { id = 1 }, request);
        }

        /// <summary>
        /// Update car details
        /// </summary>
        [HttpPut("{id}")]
        public IActionResult UpdateCar(int id, [FromBody] UpdateCarRequest request)
        {
            // TODO: Implement car update
            return Ok(new { message = "Car updated successfully" });
        }

        /// <summary>
        /// Delete a car
        /// </summary>
        [HttpDelete("{id}")]
        public IActionResult DeleteCar(int id)
        {
            // TODO: Implement car deletion
            return Ok(new { message = "Car deleted successfully" });
        }
    }

    public class CreateCarRequest
    {
        public string Make { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public string LicensePlate { get; set; }
        public int Seats { get; set; }
        public string Color { get; set; }
    }

    public class UpdateCarRequest
    {
        public string Make { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public string LicensePlate { get; set; }
        public int Seats { get; set; }
        public string Color { get; set; }
    }
}
