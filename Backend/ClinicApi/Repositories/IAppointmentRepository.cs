using ClinicApi.DTOs;
using ClinicApi.Models;

namespace ClinicApi.Repositories;

public interface IAppointmentRepository
{
    Task<int> CreateAsync(Appointment appointment);
    Task<Appointment?> GetByIdAsync(int id);
    Task<IEnumerable<AppointmentDetailDto>> GetByPatientIdAsync(int patientId);
    Task<IEnumerable<AppointmentDetailDto>> GetByDoctorIdAsync(int doctorId);
    Task<bool> UpdateStatusAsync(int id, string status);
}

