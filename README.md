# Clinic Appointment Management Application

A full-stack Clinic Appointment Management mobile application built with React Native (Expo SDK 58), ASP.NET Core Web API (.NET 9), Dapper, and PostgreSQL.

---

## 1. Project Overview

The **Clinic Appointment Management Application** facilitates seamless scheduling and management of healthcare appointments. The project is designed with a decoupled architecture ensuring clean separation of concerns between the mobile client and the backend API service.

- **Frontend**: A cross-platform mobile client built with Expo, TypeScript, and Expo Router, configured to run with Expo Go.
- **Backend**: A modular ASP.NET Core Web API utilizing Dapper and Npgsql for fast and lightweight data access without Entity Framework Core overhead.
- **Database**: Local PostgreSQL with an isolated dedicated database and schema.

---

## 2. Technology Stack

### Frontend
- **Framework**: React Native with Expo SDK 58
- **Language**: TypeScript
- **Routing**: Expo Router (File-based navigation)
- **Target Runtime**: Expo Go (Android / iOS)

### Backend
- **Framework**: ASP.NET Core Web API (.NET 9)
- **Language**: C#
- **Micro-ORM**: Dapper
- **Database Driver**: Npgsql
- **API Documentation**: Swagger / OpenAPI (Swashbuckle)

### Database
- **Engine**: Local PostgreSQL 18
- **Database**: `clinic_appointment_db`
- **Schema**: `clinic`
- **Management**: pgAdmin / psql

---

## 3. Project Structure

```text
ClinicAppointmentApp/
├── .gitignore
├── README.md
├── Backend/
│   └── ClinicApi/
│       ├── ClinicApi.csproj
│       ├── Program.cs
│       ├── appsettings.json
│       ├── appsettings.Development.json
│       ├── Controllers/
│       │   └── HealthController.cs
│       ├── Data/
│       │   ├── IDbConnectionFactory.cs
│       │   ├── DbConnectionFactory.cs
│       │   └── DatabaseInitializer.cs
│       ├── DTOs/
│       ├── Middleware/
│       ├── Models/
│       ├── Repositories/
│       └── Services/
├── Database/
│   └── Schema/
│       └── 001_create_schema.sql
├── Documentation/
│   └── architecture.md
└── Frontend/
    └── ClinicMobileApp/
        ├── app.json
        ├── babel.config.js
        ├── package.json
        ├── tsconfig.json
        ├── .gitignore
        └── app/
            ├── _layout.tsx
            └── index.tsx
```

---

## 4. PostgreSQL Configuration

The application uses an existing local PostgreSQL installation with the following parameters:

- **Host**: `localhost`
- **Port**: `5432`
- **Username**: `postgres`
- **Database**: `clinic_appointment_db`
- **Schema**: `clinic`

### Configuring the Database Password

For security, the database password is **never hardcoded** in configuration files. The backend reads it dynamically from the `DB_PASSWORD` environment variable.

#### Temporary Session (PowerShell):
```powershell
$env:DB_PASSWORD = "your_actual_password_here"
```

#### Permanent User Environment Variable (PowerShell):
```powershell
[System.Environment]::SetEnvironmentVariable("DB_PASSWORD", "your_actual_password_here", "User")
```

#### Windows Command Prompt (CMD):
```cmd
setx DB_PASSWORD "your_actual_password_here"
```

### Initial Schema Script

The schema creation script is located at:
`Database/Schema/001_create_schema.sql`

```sql
CREATE SCHEMA IF NOT EXISTS clinic;
```

When `DB_PASSWORD` is configured, the backend API automatically checks and creates `clinic_appointment_db` and the `clinic` schema on startup without affecting any other databases (such as `RenewIQ_DB`).

---

## 5. Backend Startup Commands

### Prerequisites
- .NET 9 SDK (`dotnet --version`)
- PostgreSQL running locally on port 5432
- `DB_PASSWORD` environment variable configured

### Start the API Server

1. Open PowerShell and navigate to the backend project:
   ```powershell
   cd Backend/ClinicApi
   ```

2. (Optional) Set the database password if not set globally:
   ```powershell
   $env:DB_PASSWORD = "your_actual_password_here"
   ```

3. Run the application:
   ```powershell
   dotnet run --launch-profile http
   ```

4. Access the API:
   - **Swagger UI**: [http://localhost:5290](http://localhost:5290)
   - **Health Check Endpoint**: [http://localhost:5290/api/health](http://localhost:5290/api/health)
   - **Database Health Check Endpoint**: [http://localhost:5290/api/health/database](http://localhost:5290/api/health/database)

---

## 6. Frontend Startup Commands

### Prerequisites
- Node.js (Node 22 LTS recommended for Expo SDK 58)
- Expo Go installed on your mobile device (supporting Expo SDK 58)

### Start the Mobile App

1. Navigate to the mobile app directory:
   ```powershell
   cd Frontend/ClinicMobileApp
   ```

2. Install dependencies:
   ```powershell
   npm install
   ```

3. Start the Expo development server:
   ```powershell
   npx expo start
   ```

4. Scan the generated QR code using the **Expo Go** app on your Android or iOS device.

---

## 7. Health Endpoints Reference

| Method | Endpoint | Description |
|---|---|---|
| `GET` | `/api/health` | Returns API service uptime and health status (`200 OK`). |
| `GET` | `/api/health/database` | Verifies PostgreSQL connectivity using Dapper and queries active database/schema. Returns `200 OK` when healthy, or `503 Service Unavailable` if `DB_PASSWORD` is unset or PostgreSQL is unreachable. |

