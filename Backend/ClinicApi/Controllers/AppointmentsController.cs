using ClinicApi.DTOs;
using ClinicApi.Models;
using ClinicApi.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ClinicApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AppointmentsController : ControllerBase
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IUserRepository _userRepository;

    public AppointmentsController(IAppointmentRepository appointmentRepository, IUserRepository userRepository)
    {
        _appointmentRepository = appointmentRepository;
        _userRepository = userRepository;
    }

    /// <summary>
    /// Book a new appointment (Status defaults to PENDING).
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> BookAppointment([FromBody] BookAppointmentRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var patient = await _userRepository.GetByIdAsync(request.PatientId);
        if (patient == null || patient.Role != "PATIENT")
            return BadRequest(new { message = "Invalid patient ID." });

        var doctor = await _userRepository.GetByIdAsync(request.DoctorId);
        if (doctor == null || doctor.Role != "DOCTOR")
            return BadRequest(new { message = "Invalid doctor ID." });

        var appointment = new Appointment
        {
            PatientId = request.PatientId,
            DoctorId = request.DoctorId,
            AppointmentDate = request.AppointmentDate.Date,
            TimeSlot = request.TimeSlot.Trim(),
            Reason = request.Reason.Trim(),
            Status = "PENDING"
        };

        var id = await _appointmentRepository.CreateAsync(appointment);
        appointment.Id = id;

        return CreatedAtAction(nameof(GetAppointmentById), new { id = appointment.Id }, appointment);
    }

    /// <summary>
    /// Get single appointment by ID.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetAppointmentById(int id)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(id);
        if (appointment == null)
            return NotFound(new { message = "Appointment not found." });

        return Ok(appointment);
    }

    /// <summary>
    /// Get all appointments for a patient.
    /// </summary>
    [HttpGet("patient/{patientId}")]
    public async Task<IActionResult> GetPatientAppointments(int patientId)
    {
        var appointments = await _appointmentRepository.GetByPatientIdAsync(patientId);
        return Ok(appointments);
    }

    /// <summary>
    /// Get all appointments assigned to a doctor.
    /// </summary>
    [HttpGet("doctor/{doctorId}")]
    public async Task<IActionResult> GetDoctorAppointments(int doctorId)
    {
        var appointments = await _appointmentRepository.GetByDoctorIdAsync(doctorId);
        return Ok(appointments);
    }

    /// <summary>
    /// Update appointment status (ACCEPTED, REJECTED, CANCELLED).
    /// </summary>
    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateAppointmentStatusRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var validStatuses = new[] { "ACCEPTED", "REJECTED", "CANCELLED" };
        var upperStatus = request.Status.ToUpper().Trim();

        if (!validStatuses.Contains(upperStatus))
        {
            return BadRequest(new { message = $"Status must be one of: {string.Join(", ", validStatuses)}" });
        }

        var existing = await _appointmentRepository.GetByIdAsync(id);
        if (existing == null)
            return NotFound(new { message = "Appointment not found." });

        var updated = await _appointmentRepository.UpdateStatusAsync(id, upperStatus);
        if (!updated)
            return StatusCode(500, new { message = "Failed to update appointment status." });

        return Ok(new
        {
            id,
            status = upperStatus,
            message = $"Appointment status updated to {upperStatus} successfully."
        });
    }
}

