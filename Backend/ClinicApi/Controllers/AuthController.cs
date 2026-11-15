using ClinicApi.DTOs;
using ClinicApi.Models;
using ClinicApi.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ClinicApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserRepository _userRepository;

    public AuthController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user == null || user.Password != request.Password)
        {
            return Unauthorized(new { message = "Invalid email or password." });
        }

        var response = new LoginResponse
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role,
            Specialty = user.Specialty
        };

        return Ok(response);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var existing = await _userRepository.GetByEmailAsync(request.Email);
        if (existing != null)
        {
            return Conflict(new { message = "User with this email already exists." });
        }

        var newUser = new User
        {
            Name = request.Name.Trim(),
            Email = request.Email.Trim().ToLower(),
            Password = request.Password,
            Role = request.Role.ToUpper().Trim(),
            Specialty = request.Role.ToUpper() == "DOCTOR" ? request.Specialty : null
        };

        var newId = await _userRepository.CreateAsync(newUser);
        newUser.Id = newId;

        return Ok(new LoginResponse
        {
            Id = newUser.Id,
            Name = newUser.Name,
            Email = newUser.Email,
            Role = newUser.Role,
            Specialty = newUser.Specialty
        });
    }

    [HttpGet("doctors")]
    public async Task<IActionResult> GetDoctors()
    {
        var doctors = await _userRepository.GetDoctorsAsync();
        return Ok(doctors.Select(d => new
        {
            d.Id,
            d.Name,
            d.Email,
            d.Role,
            d.Specialty
        }));
    }
}

