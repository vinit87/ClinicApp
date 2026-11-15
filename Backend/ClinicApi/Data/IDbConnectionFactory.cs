using System.Data;

namespace ClinicApi.Data;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
    IDbConnection CreateServerConnection();
    bool IsPasswordConfigured { get; }
    string DatabaseName { get; }
    string SchemaName { get; }
}

