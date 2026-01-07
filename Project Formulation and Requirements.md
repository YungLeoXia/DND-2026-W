# CarGoCar

## The Backstory

This project is a simplified ride-sharing web application, focused on coordinating carpool trips. The app lets drivers publish rides (including route, stops, timing, available seats, and price) and lets users reserve seats on those rides.

The system supports three distinct actors: Admins, Drivers, and Users/Passengers. Drivers can list their cars and publish rides; users can search rides and reserve seats. Due to time constraints and complexity, the platform deliberately excludes payment processing and instead focuses on core features such as ride publishing, seat reservations, authentication, authorization, and data management.

## Users

- Passengers / Users
- Drivers
- Admins

## Requirements

### Visitor

- As a visitor, I want to register an account so I can use the application.

### User / Passenger

- As a user, I want to log in and log out so my account is protected.
- As a user, I want to view and update my profile so my information stays current.
- As a user, I want to search for rides by location and date so I can find suitable trips.
- As a user, I want to view ride details including driver information, stops, and remaining seats.
- As a user, I want to reserve one or more seats on a ride so I can join it.
- As a user, I want to cancel my reservation so seats are released for other users.

### Driver

- As a driver, I want to register a car with seat capacity so I can offer rides.
- As a driver, I want to view and manage my cars so I can reuse them for different rides.
- As a driver, I want to create a ride using one of my cars so passengers can reserve seats.
- As a driver, I want to define ride details such as route, departure time, duration, price, and available seats.
- As a driver, I want to add optional stops to a ride so passengers can join from additional locations.
- As a driver, I want to update or cancel a ride if my plans change.

### Admin

- As an admin, I want to view all users so I can monitor platform activity.
- As an admin, I want to disable user accounts so I can moderate the platform.
- As an admin, I want to remove ride listings that violate platform rules.
