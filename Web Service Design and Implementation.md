
## 3. Web Service Design and Implementation

### API Overview

The backend of CarGoCar is implemented as a **RESTful Web API** using ASP.NET Core. It acts as the central communication layer between the Blazor frontend and the database.

### Endpoint Design

Endpoints are grouped by responsibility (auth, users, rides, reservations, admin), and standard HTTP verbs are used consistently. For example, ride searching is handled via query parameters:

```csharp
[HttpGet]
public async Task<IActionResult> GetRides(string? from, string? to, DateTime? date)
```

### Authorization and Validation

Before performing sensitive actions, endpoints validate both the request data and the caller’s identity. A typical example is preventing users from booking their own rides or exceeding available seats.

```csharp
if (ride.DriverId == passengerId)
    return BadRequest(new { error = "Cannot book your own ride" });
```

### Process Reflection

Designing the API was an iterative process. Early versions were simpler, but as edge cases emerged (such as cancellation or confirmation flows), endpoints were refined to better reflect real-world usage. Testing was done using `.http` files and Swagger, which made it easy to manually verify request–response behaviour.

