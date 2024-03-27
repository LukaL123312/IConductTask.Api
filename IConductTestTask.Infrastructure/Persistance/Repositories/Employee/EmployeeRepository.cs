using IConductTestTask.Application.Common.Interfaces.Repositories.Employee;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;

namespace IConductTestTask.Infrastructure.Persistance.Repositories.Employee;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly IConfiguration _configuration;

    public EmployeeRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task EnableEmployee(int employeeId)
    {
        using (var connection = new SqlConnection(_configuration.GetConnectionString("AppDbConnection")))
        {
            await connection.OpenAsync();

            connection.ChangeDatabase(_configuration["DatabaseName"]);

            var commandText = "UPDATE Employee SET Enabled = 1 WHERE Id = @Id";

            using (var command = new SqlCommand(commandText, connection))
            {
                command.Parameters.AddWithValue("@Id", employeeId);
                await command.ExecuteNonQueryAsync();
            }
        }
    }

    public async Task<Domain.Employee?> GetById(int employeeId)
    {
        using (var connection = new SqlConnection(_configuration.GetConnectionString("AppDbConnection")))
        {
            await connection.OpenAsync();

            connection.ChangeDatabase(_configuration["DatabaseName"]);

            var commandText = "SELECT Id, Name, ManagerId, Enabled FROM Employee WHERE Id = @Id";

            using (var command = new SqlCommand(commandText, connection))
            {
                command.Parameters.AddWithValue("@Id", employeeId);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        if (!reader.GetBoolean(reader.GetOrdinal("Enabled")))
                        {
                            return null; // If not enabled, return null
                        }

                        return new Domain.Employee
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            ManagerId = reader.IsDBNull(reader.GetOrdinal("ManagerId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("ManagerId")),
                            Enabled = reader.GetBoolean(reader.GetOrdinal("Enabled"))
                        };
                    }

                    return null;
                }
            }
        }
    }

    public async Task<bool> CheckIfEmployeeIsManager(int employeeId)
    {
        using (var connection = new SqlConnection(_configuration.GetConnectionString("AppDbConnection")))
        {
            await connection.OpenAsync();

            connection.ChangeDatabase(_configuration["DatabaseName"]);

            var commandText = "SELECT COUNT(*) FROM Manager WHERE EmployeeId = @EmployeeId";

            using (var command = new SqlCommand(commandText, connection))
            {
                command.Parameters.AddWithValue("@EmployeeId", employeeId);

                int count = Convert.ToInt32(await command.ExecuteScalarAsync());

                return count > 0;
            }
        }
    }

    public async Task<Domain.Employee?> GetFullEmployeeTree(int employeeId)
    {
        var employee = await GetById(employeeId);

        if (employee is null)
        {
            return null;
        }

        if (!await CheckIfEmployeeIsManager(employeeId))
        {
            return employee;
        }

        var supervisedEmployees = await GetSupervisedEmployees(employeeId);

        foreach (var supervisedEmployee in supervisedEmployees)
        {
            supervisedEmployee.SupervisedEmployees = await GetSupervisedEmployees(supervisedEmployee.Id);
        }

        employee.SupervisedEmployees = supervisedEmployees;

        return employee;
    }

    private async Task<List<Domain.Employee>> GetSupervisedEmployees(int managerEmployeeId)
    {
        var supervisedEmployees = new List<Domain.Employee>();

        using (var connection = new SqlConnection(_configuration.GetConnectionString("AppDbConnection")))
        {
            await connection.OpenAsync();

            connection.ChangeDatabase(_configuration["DatabaseName"]);

            var commandText = "SELECT Id, Name, ManagerId, Enabled FROM Employee WHERE ManagerId = @ManagerId";

            using (var command = new SqlCommand(commandText, connection))
            {
                command.Parameters.AddWithValue("@ManagerId", managerEmployeeId);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        if (!reader.GetBoolean(reader.GetOrdinal("Enabled")))
                        {
                            continue;
                        }

                        supervisedEmployees.Add(new Domain.Employee
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            ManagerId = reader.IsDBNull(reader.GetOrdinal("ManagerId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("ManagerId")),
                            Enabled = reader.GetBoolean(reader.GetOrdinal("Enabled"))
                        });
                    }
                }
            }
        }

        return supervisedEmployees;
    }
}
