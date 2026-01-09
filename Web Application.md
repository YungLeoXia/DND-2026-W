
## 4. Web Application (Frontend)

### Technology Choice

The frontend is built using **Blazor**, allowing the entire stack to be written in C#. This reduced context switching and made it easier to share mental models between frontend and backend development.

### API Communication

All API calls are routed through a small abstraction layer (`ApiService`), which wraps `HttpClient` and exposes generic helper methods.

```csharp
await _http.GetFromJsonAsync<T>("/api/" + endpoint);
```

This kept Razor components focused on presentation logic rather than networking details.

### Authentication in the UI

The `AuthService` manages authentication state and exposes convenience properties such as `IsAdmin` or `IsDriver`. Razor components subscribe to authentication changes to update navigation and available actions dynamically.

### Process Reflection

The frontend was developed incrementally, starting with basic authentication and ride listing, and later expanding to reservations and admin views. This approach helped keep the UI functional at all times, even while backend features were still evolving.
