

## 1. User Management

### Overview

User management forms the foundation of the CarGoCar application. Since the platform supports multiple actors with different responsibilities, a clear separation of roles and permissions was essential from the beginning. The system distinguishes between **Visitors**, **Users/Passengers**, **Drivers**, and **Admins**, each with access to a different subset of features.

### Roles and Responsibilities

* **Visitor**: An unregistered user who can only access public pages and register an account.
* **User / Passenger**: Can log in, manage a profile, search for rides, view ride details, reserve seats, and cancel reservations.
* **Driver**: Inherits user functionality and can additionally register cars, create rides, define routes and stops, and manage published rides.
* **Admin**: Has elevated privileges for moderation, including viewing all users, disabling accounts, and removing inappropriate ride listings.

### Authentication and Authorization

Authentication is implemented using **JWT (JSON Web Tokens)**. When a user logs in, the backend generates a token containing the user’s ID and role as claims. This token is then attached to all subsequent API requests from the frontend.

```csharp
new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
new Claim(ClaimTypes.Role, user.Role)
```

On the API side, authorization is enforced declaratively using role attributes on controllers and endpoints, for example restricting certain actions to drivers or admins only.

```csharp
[Authorize(Roles = "Driver,Admin")]
```

On the frontend, the authentication state is managed through a dedicated `AuthService`, which stores the token in memory and injects it into the `HttpClient` authorization header. This allowed UI components to react to authentication changes without complex state management libraries.

### Process Reflection

User management was implemented early in the project, as most other features depend on a reliable identity and role system. This decision helped avoid repeated refactoring later and made it easier to reason about permissions while implementing rides and reservations.


