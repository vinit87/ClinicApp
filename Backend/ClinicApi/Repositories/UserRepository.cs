using ClinicApi.Data;
using ClinicApi.Models;
using Dapper;

namespace ClinicApi.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public UserRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        const string sql = @"
            SELECT id, name, email, password, role, specialty, created_at AS CreatedAt
            FROM clinic.users
            WHERE LOWER(email) = LOWER(@Email);";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<User>(sql, new { Email = email.Trim() });
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        const string sql = @"
            SELECT id, name, email, password, role, specialty, created_at AS CreatedAt
            FROM clinic.users
            WHERE id = @Id;";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<User>(sql, new { Id = id });
    }

    public async Task<IEnumerable<User>> GetDoctorsAsync()
    {
        const string sql = @"
            SELECT id, name, email, '' as password, role, specialty, created_at AS CreatedAt
            FROM clinic.users
            WHERE role = 'DOCTOR'
            ORDER BY name ASC;";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryAsync<User>(sql);
    }

    public async Task<int> CreateAsync(User user)
    {
        const string sql = @"
            INSERT INTO clinic.users (name, email, password, role, specialty)
            VALUES (@Name, @Email, @Password, @Role, @Specialty)
            RETURNING id;";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<int>(sql, user);
    }
}

