using ClinicApi.Models;

namespace ClinicApi.Repositories;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByIdAsync(int id);
    Task<IEnumerable<User>> GetDoctorsAsync();
    Task<int> CreateAsync(User user);
}

