
## 2. Data Access Layer

### Purpose and Scope

The data access layer is responsible for managing persistent data and ensuring consistency between the application logic and the database. In CarGoCar, this layer is implemented using **Entity Framework Core**, providing object-relational mapping (ORM) between C# models and a SQLite database.

### Database Design

The database design closely mirrors the core domain:

* **Users** with roles and profile data
* **Cars** owned by drivers
* **Rides** linked to a specific car and driver
* **Reservations** connecting passengers to rides
* **Stops** representing optional intermediate locations

Relationships are explicitly configured using the Fluent API in `AppDbContext`, ensuring referential integrity.

```csharp
entity.HasMany(u => u.Cars)
      .WithOne(c => c.Driver)
      .HasForeignKey(c => c.DriverId);
```

### Business Rules Remembered in Data Logic

Some important business constraints are enforced at the data and controller level together, such as:

* Seat availability being decreased when a reservation is created
* Seats being restored if a reservation is cancelled

```csharp
ride.AvailableSeats -= request.NumberOfSeats;
```

This approach ensured that the database always reflected the current system state, even if multiple operations were performed in sequence.

### Process Reflection

Using EF Core migrations and seeding helped speed up development and testing. While SQLite is not suitable for large-scale production systems, it was a pragmatic choice for a student project due to its simplicity and low configuration overhead.

