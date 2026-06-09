# 🏨 Hotel Booking Platform

![.NET Core](https://img.shields.io/badge/.NET%2010-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![Angular](https://img.shields.io/badge/Angular_21-%23DD0031.svg?style=for-the-badge&logo=angular&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL_Server-CC2927?style=for-the-badge&logo=microsoft-sql-server&logoColor=white)
![Entity Framework Core](https://img.shields.io/badge/EF_Core-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![Bootstrap](https://img.shields.io/badge/Bootstrap_5-7952B3?style=for-the-badge&logo=bootstrap&logoColor=white)

A full-stack, multi-tenant Hotel Booking Management System designed with a focus on enterprise-level architecture, scalability, and security. Built using **ASP.NET Core 8 Web API** with a strictly enforced **Clean Architecture (Domain-Driven Design)** backend and a modern, highly responsive **Angular 17+** frontend.

## ✨ Key Features

- **Role-Based Access Control (RBAC):** Three distinct roles with heavily customized dashboards and API authorizations:
  - `SuperAdmin`: Global platform oversight, system management, and analytics.
  - `HotelAdmin`: Dedicated access strictly scoped to the hotel they manage (inventory, bookings, staff).
  - `Customer`: Browse hotels, book rooms, leave reviews, and manage booking history.
- **Robust Authentication:** Secure JWT (JSON Web Token) bearer authentication with local .NET User Secrets configuration.
- **End-to-End Booking Engine:** Real-time room availability, automatic total price calculations, and seamless checkout flows.
- **Automated Email Notifications:** Integrated SMTP service to dispatch automated booking confirmations and updates.
- **Advanced Logging & Monitoring:** Configured with `Serilog` for structured request and error logging.

---

## 🏗 Architecture & Design Patterns

### Backend (Clean Architecture / DDD)
The `.NET 10` solution is heavily decoupled into 4 distinct layers:
1. **Domain Layer:** Core business entities (`Hotel`, `Room`, `Booking`, `Payment`) and strictly defined Enums.
2. **Application Layer:** DTOs, Validation, and strongly-typed Services enforcing business logic (e.g., preventing double-booking).
3. **Infrastructure Layer:** Entity Framework Core `DbContext`, Data Seeding, and implementations of the **Repository** and **Unit of Work** design patterns for atomic SQL transactions.
4. **API Layer:** Controllers, global exception handling middleware, and JWT configurations.

### Frontend (Angular 21)
- **Standalone Components:** Leverages the latest Angular paradigms without `NgModules`.
- **Functional Interceptors & Guards:** Uses functional routing guards to protect dashboard routes and HTTP interceptors to automatically inject JWTs.
- **Modular Design:** Features are split into lazy-loaded, domain-specific modules (`auth`, `public`, `customer`, `hotel-admin`, `super-admin`) to ensure minimal initial bundle sizes and fast First Contentful Paint (FCP).

---

## 🚀 Getting Started

### Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Node.js (v18+)](https://nodejs.org/)
- SQL Server (LocalDB or Express)

### 1. Setting up the Backend
1. Navigate to the API folder:
   ```bash
   cd Backend/HotelBooking/HotelBooking.API
   ```
2. Initialize User Secrets for the SMTP Email service (Optional but recommended):
   ```bash
   dotnet user-secrets init
   dotnet user-secrets set "EmailSettings:SenderEmail" "your-email@gmail.com"
   dotnet user-secrets set "EmailSettings:Password" "your-app-password"
   ```
3. Run Entity Framework Migrations and seed the database:
   ```bash
   dotnet ef database update
   ```
4. Run the API:
   ```bash
   dotnet run
   ```
   *The Swagger UI will be available at `https://localhost:7106/swagger`*

### 2. Setting up the Frontend
1. Open a new terminal and navigate to the Frontend folder:
   ```bash
   cd Frontend
   ```
2. Install dependencies:
   ```bash
   npm install
   ```
3. Start the Angular Development Server:
   ```bash
   npm start
   ```
   *The application will automatically launch at `http://localhost:4200`*

---

## 🔐 Default Test Accounts (Data Seeder)

When the application runs for the first time, the `DataSeeder` automatically populates the SQL database with mock data. You can log in using the following credentials:

| Role | Email | Password |
| :--- | :--- | :--- |
| **Super Admin** | `admin@hotelbooking.com` | `Admin@123` |
| **Hotel Admin** | `manager1@grandhotel.com` | `Admin@123` |
| **Customer** | `john.doe@example.com` | `Password@123` |

---

## 🛠 Future Roadmap
- Integration of a 3rd party Payment Gateway (Stripe / PayPal).
- Real-time notifications for Hotel Admins using SignalR.
- Advanced Analytics dashboard using Chart.js.

---
*This project was developed to demonstrate full-stack proficiency, architectural best practices, and enterprise system design.*
