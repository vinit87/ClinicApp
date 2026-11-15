using System.ComponentModel.DataAnnotations;

namespace ClinicApi.DTOs;

public class BookAppointmentRequest
{
    [Required]
    public int PatientId { get; set; }

    [Required]
    public int DoctorId { get; set; }

    [Required]
    public DateTime AppointmentDate { get; set; }

    [Required]
    public string TimeSlot { get; set; } = string.Empty;

    [Required]
    public string Reason { get; set; } = string.Empty;
}

public class UpdateAppointmentStatusRequest
{
    [Required]
    public string Status { get; set; } = string.Empty; // "ACCEPTED", "REJECTED", "CANCELLED"
}

public class AppointmentDetailDto
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string PatientEmail { get; set; } = string.Empty;
    public int DoctorId { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public string? DoctorSpecialty { get; set; }
    public DateTime AppointmentDate { get; set; }
    public string TimeSlot { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
