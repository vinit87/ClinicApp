namespace ClinicApi.Models;

public class Appointment
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public int DoctorId { get; set; }
    public DateTime AppointmentDate { get; set; }
    public string TimeSlot { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public string Status { get; set; } = "PENDING"; // "PENDING", "ACCEPTED", "REJECTED", "CANCELLED"
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
