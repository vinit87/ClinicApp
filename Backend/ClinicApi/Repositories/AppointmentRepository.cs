using ClinicApi.Data;
using ClinicApi.DTOs;
using ClinicApi.Models;
using Dapper;

namespace ClinicApi.Repositories;

public class AppointmentRepository : IAppointmentRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public AppointmentRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<int> CreateAsync(Appointment appointment)
    {
        const string sql = @"
            INSERT INTO clinic.appointments (patient_id, doctor_id, appointment_date, time_slot, reason, status)
            VALUES (@PatientId, @DoctorId, @AppointmentDate, @TimeSlot, @Reason, 'PENDING')
            RETURNING id;";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<int>(sql, appointment);
    }

    public async Task<Appointment?> GetByIdAsync(int id)
    {
        const string sql = @"
            SELECT id, patient_id AS PatientId, doctor_id AS DoctorId, appointment_date AS AppointmentDate,
                   time_slot AS TimeSlot, reason, status, created_at AS CreatedAt, updated_at AS UpdatedAt
            FROM clinic.appointments
            WHERE id = @Id;";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<Appointment>(sql, new { Id = id });
    }

    public async Task<IEnumerable<AppointmentDetailDto>> GetByPatientIdAsync(int patientId)
    {
        const string sql = @"
            SELECT a.id, 
                   a.patient_id AS PatientId, 
                   p.name AS PatientName, 
                   p.email AS PatientEmail,
                   a.doctor_id AS DoctorId, 
                   d.name AS DoctorName, 
                   d.specialty AS DoctorSpecialty,
                   a.appointment_date AS AppointmentDate, 
                   a.time_slot AS TimeSlot, 
                   a.reason, 
                   a.status, 
                   a.created_at AS CreatedAt
            FROM clinic.appointments a
            INNER JOIN clinic.users p ON a.patient_id = p.id
            INNER JOIN clinic.users d ON a.doctor_id = d.id
            WHERE a.patient_id = @PatientId
            ORDER BY a.appointment_date DESC, a.created_at DESC;";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryAsync<AppointmentDetailDto>(sql, new { PatientId = patientId });
    }

    public async Task<IEnumerable<AppointmentDetailDto>> GetByDoctorIdAsync(int doctorId)
    {
        const string sql = @"
            SELECT a.id, 
                   a.patient_id AS PatientId, 
                   p.name AS PatientName, 
                   p.email AS PatientEmail,
                   a.doctor_id AS DoctorId, 
                   d.name AS DoctorName, 
                   d.specialty AS DoctorSpecialty,
                   a.appointment_date AS AppointmentDate, 
                   a.time_slot AS TimeSlot, 
                   a.reason, 
                   a.status, 
                   a.created_at AS CreatedAt
            FROM clinic.appointments a
            INNER JOIN clinic.users p ON a.patient_id = p.id
            INNER JOIN clinic.users d ON a.doctor_id = d.id
            WHERE a.doctor_id = @DoctorId
            ORDER BY a.appointment_date ASC, a.created_at ASC;";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryAsync<AppointmentDetailDto>(sql, new { DoctorId = doctorId });
    }

    public async Task<bool> UpdateStatusAsync(int id, string status)
    {
        const string sql = @"
            UPDATE clinic.appointments
            SET status = @Status, updated_at = CURRENT_TIMESTAMP
            WHERE id = @Id;";

        using var connection = _connectionFactory.CreateConnection();
        var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id, Status = status.ToUpper().Trim() });
        return rowsAffected > 0;
    }
}

