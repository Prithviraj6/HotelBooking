# HotelBooking

HotelBooking is a layered ASP.NET Core backend for a hotel booking system. The current repository contains the backend solution and an empty `Frontend` folder reserved for a future client application.

## Project Structure

```text
Backend/
  HotelBooking/
    HotelBooking.slnx
    HotelBooking.API/             ASP.NET Core API, controllers, middleware, app startup
    HotelBooking.Application/     DTOs, service interfaces, application services
    HotelBooking.Domain/          Entities, enums, repository contracts, shared domain types
    HotelBooking.Infrastructure/  Data access, EF configurations, repositories, unit of work
Frontend/                         Placeholder for the frontend application
```

## Requirements

- .NET SDK compatible with `net10.0`
- Git

## Run the Backend

From the repository root:

```powershell
dotnet restore Backend\HotelBooking\HotelBooking.slnx
dotnet build Backend\HotelBooking\HotelBooking.slnx
dotnet run --project Backend\HotelBooking\HotelBooking.API\HotelBooking.API.csproj
```

## GitHub Repository Notes

The repository includes:

- `.gitignore` for .NET build output, IDE metadata, local secrets, frontend dependencies, and temporary files.
- `.gitattributes` for consistent text handling across operating systems.
- `README.md` with the initial project overview and run commands.

Generated folders such as `bin/`, `obj/`, and `.vs/` should stay out of Git.
