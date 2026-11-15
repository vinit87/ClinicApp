# Architecture Documentation - Clinic Appointment App

## Overview
The Clinic Appointment Application is architected with a decoupled mobile-first frontend and high-performance RESTful backend:

- **Mobile Client**: React Native with Expo SDK 58, Expo Router, and TypeScript, optimized for Expo Go.
- **Backend API**: ASP.NET Core Web API (.NET 9) leveraging Dapper and Npgsql for raw SQL performance and low-overhead object mapping.
- **Database**: Local PostgreSQL 18 instance with a dedicated database (`clinic_appointment_db`) and schema (`clinic`).

## Architectural Principles
1. **No ORM Overhead**: Entity Framework Core is strictly prohibited in favor of Dapper + Npgsql.
2. **Database Isolation**: The application strictly operates inside its own database (`clinic_appointment_db`) and schema (`clinic`), protecting existing databases (e.g. `RenewIQ_DB`).
3. **Secret Security**: Database credentials are strictly sourced via the `DB_PASSWORD` environment variable and never committed to version control.
4. **Decoupled Repositories**: Mobile application and backend API operate independently and can be scaled, tested, or deployed in isolation.

