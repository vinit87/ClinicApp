using Dapper;

namespace ClinicApi.Data;

public class DatabaseInitializer
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ILogger<DatabaseInitializer> _logger;

    public DatabaseInitializer(IDbConnectionFactory connectionFactory, ILogger<DatabaseInitializer> logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
    }

    public async Task InitializeAsync()
    {
        if (!_connectionFactory.IsPasswordConfigured)
        {
            _logger.LogWarning("DB_PASSWORD is not set. Skipping automatic database creation and table provisioning.");
            return;
        }

        try
        {
            // 1. Ensure target database 'clinic_appointment_db' exists on PostgreSQL server
            using (var serverConn = _connectionFactory.CreateServerConnection())
            {
                serverConn.Open();
                var dbName = _connectionFactory.DatabaseName;

                var exists = await serverConn.ExecuteScalarAsync<int?>(
                    "SELECT 1 FROM pg_database WHERE datname = @dbName;",
                    new { dbName });

                if (exists != 1)
                {
                    _logger.LogInformation("Database '{DatabaseName}' does not exist. Creating database...", dbName);
                    await serverConn.ExecuteAsync($"CREATE DATABASE \"{dbName}\";");
                    _logger.LogInformation("Database '{DatabaseName}' created successfully.", dbName);
                }
            }

            // 2. Ensure schema and tables exist in clinic_appointment_db
            using (var dbConn = _connectionFactory.CreateConnection())
            {
                dbConn.Open();
                var schema = _connectionFactory.SchemaName;

                _logger.LogInformation("Ensuring schema '{SchemaName}' and application tables exist...", schema);
                await dbConn.ExecuteAsync($"CREATE SCHEMA IF NOT EXISTS \"{schema}\";");

                // Create users table
                await dbConn.ExecuteAsync(@"
                    CREATE TABLE IF NOT EXISTS clinic.users (
                        id SERIAL PRIMARY KEY,
                        name VARCHAR(100) NOT NULL,
                        email VARCHAR(150) UNIQUE NOT NULL,
                        password VARCHAR(100) NOT NULL,
                        role VARCHAR(20) NOT NULL CHECK (role IN ('PATIENT', 'DOCTOR')),
                        specialty VARCHAR(100),
                        created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
                    );
                ");

                // Create appointments table
                await dbConn.ExecuteAsync(@"
                    CREATE TABLE IF NOT EXISTS clinic.appointments (
                        id SERIAL PRIMARY KEY,
                        patient_id INT NOT NULL REFERENCES clinic.users(id) ON DELETE CASCADE,
                        doctor_id INT NOT NULL REFERENCES clinic.users(id) ON DELETE CASCADE,
                        appointment_date DATE NOT NULL,
                        time_slot VARCHAR(50) NOT NULL,
                        reason TEXT NOT NULL,
                        status VARCHAR(20) NOT NULL DEFAULT 'PENDING' CHECK (status IN ('PENDING', 'ACCEPTED', 'REJECTED', 'CANCELLED')),
                        created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                        updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
                    );
                ");

                // Seed initial demo doctors and patients
                await dbConn.ExecuteAsync(@"
                    INSERT INTO clinic.users (name, email, password, role, specialty)
                    VALUES 
                        ('Dr. Sarah Smith', 'dr.sarah@clinic.com', 'doctor123', 'DOCTOR', 'General Physician'),
                        ('Dr. James Wilson', 'dr.james@clinic.com', 'doctor123', 'DOCTOR', 'Cardiologist'),
                        ('Dr. Emily Taylor', 'dr.emily@clinic.com', 'doctor123', 'DOCTOR', 'Dentist'),
                        ('Alex Johnson', 'alex@patient.com', 'patient123', 'PATIENT', NULL),
                        ('Maria Garcia', 'maria@patient.com', 'patient123', 'PATIENT', NULL)
                    ON CONFLICT (email) DO NOTHING;
                ");

                // Seed sample appointment
                await dbConn.ExecuteAsync(@"
                    INSERT INTO clinic.appointments (patient_id, doctor_id, appointment_date, time_slot, reason, status)
                    SELECT p.id, d.id, CURRENT_DATE + INTERVAL '1 day', '10:00 AM', 'Routine consultation and wellness checkup', 'PENDING'
                    FROM clinic.users p, clinic.users d
                    WHERE p.email = 'alex@patient.com' AND d.email = 'dr.sarah@clinic.com'
                    AND NOT EXISTS (
                        SELECT 1 FROM clinic.appointments 
                        WHERE patient_id = p.id AND doctor_id = d.id AND reason = 'Routine consultation and wellness checkup'
                    );
                ");

                _logger.LogInformation("Database tables and seed data initialized successfully.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Database initialization encountered an error: {Message}", ex.Message);
        }
    }
}
