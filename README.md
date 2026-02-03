# PharmacyManager

ASP.NET Core MVC application for managing pharmacy operations.

## Tech Stack

- .NET 9.0
- Entity Framework Core (SQL Server)

## How to Run

1. Update the connection string in `appsettings.json`
2. Run the SQL script in `SQL/DatabaseSetup.sql`
3. Start the app:

```
dotnet run
```

4. Open `https://localhost:5001` in your browser
5. Log in to access the system

## Demo Login

- Username: `admin`
- Password: `admin123`

## Features

- User login / session management
- Dashboard
- Inventory management (CRUD)
- Billing (create, view, details)
