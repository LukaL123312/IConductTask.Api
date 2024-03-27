using IConductTestTask.Application.Common.Interfaces.Repositories.Employee;
using IConductTestTask.Infrastructure.Persistance.Repositories.Employee;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IConductTestTask.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDatabaseRelatedServices(configuration);

        return services;
    }

    private static void AddDatabaseRelatedServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();

        var connectionString = configuration.GetConnectionString("AppDbConnection");

        var databaseName = configuration["DatabaseName"];

        CreateDatabase(connectionString, databaseName);

        CreateTable(connectionString, databaseName, "Employee", "Id INT IDENTITY(1,1) PRIMARY KEY, Name NVARCHAR(100), ManagerId INT NULL, Enabled BIT");
        CreateTable(connectionString, databaseName, "Manager", "Id INT IDENTITY(1,1) PRIMARY KEY, EmployeeId INT");

        DefineOneToManyRelationship(connectionString, "Employee", "Manager", "ManagerId", "Id");

        SeedEmployeeData(connectionString, databaseName);

        SeedManagerData(connectionString, databaseName);
    }

    public static void CreateDatabase(string connectionString, string databaseName)
    {
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();

            var commandText = $"IF NOT EXISTS (SELECT 1 FROM sys.databases WHERE name = '{databaseName}') CREATE DATABASE [{databaseName}]";
            using (var command = new SqlCommand(commandText, connection))
            {
                command.ExecuteNonQuery();
            }
        }
    }

    public static void CreateTable(string connectionString, string databaseName, string tableName, string tableDefinition)
    {
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();

            connection.ChangeDatabase(databaseName);

            var commandText = $"IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = '{tableName}') CREATE TABLE [{tableName}] ({tableDefinition})";
            using (var command = new SqlCommand(commandText, connection))
            {
                command.ExecuteNonQuery();
            }
        }
    }

    public static void DefineOneToManyRelationship(string connectionString,
        string referencedTableName, string referencerTableName,
        string foreignKeyColumnName, string referencedColumnName)
    {
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();


            var commandText = $@"SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
                     WHERE CONSTRAINT_TYPE = 'FOREIGN KEY' 
                     AND TABLE_NAME = '{referencedTableName}' 
                     AND CONSTRAINT_NAME = 'FK_{referencedTableName}_{foreignKeyColumnName}'";

            using (var command = new SqlCommand(commandText, connection))
            {
                int count = Convert.ToInt32(command.ExecuteScalar());

                if (count > 0)
                {
                    return;
                }
            }

            commandText = $@"ALTER TABLE {referencedTableName} 
                        ADD CONSTRAINT FK_{referencedTableName}_{foreignKeyColumnName} 
                        FOREIGN KEY ({foreignKeyColumnName}) 
                        REFERENCES {referencerTableName} ({referencedColumnName})";

            using (var command = new SqlCommand(commandText, connection))
            {
                command.ExecuteNonQuery();
            }
        }
    }

    public static void SeedEmployeeData(string connectionString, string databaseName)
    {
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();

            connection.ChangeDatabase(databaseName);

            var selectCommandText = "SELECT COUNT(*) FROM Employee";
            using (var command = new SqlCommand(selectCommandText, connection))
            {
                int employeeCount = Convert.ToInt32(command.ExecuteScalar());
                if (employeeCount > 5)
                {
                    // Exit the method if there are already more than 5 records
                    return;
                }
            }

            // Seed data into Employee table
            var insertCommandText = "INSERT INTO Employee (Name, ManagerId, Enabled) VALUES (@Name, @ManagerId, @Enabled)";
            using (var command = new SqlCommand(insertCommandText, connection))
            {
                // Employee 1: No manager
                command.Parameters.AddWithValue("@Name", "Employee 1");
                command.Parameters.AddWithValue("@ManagerId", DBNull.Value);
                command.Parameters.AddWithValue("@Enabled", true);
                command.ExecuteNonQuery();

                // Employee 2: Manager is Employee 1
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@Name", "Employee 2");
                command.Parameters.AddWithValue("@ManagerId", 1); // ManagerId is Employee 1
                command.Parameters.AddWithValue("@Enabled", true);
                command.ExecuteNonQuery();

                // Employee 3: Manager is Employee 2
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@Name", "Employee 3");
                command.Parameters.AddWithValue("@ManagerId", 2); // ManagerId is Employee 2
                command.Parameters.AddWithValue("@Enabled", true);
                command.ExecuteNonQuery();

                // Employee 4: Manager is Employee 1
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@Name", "Employee 4");
                command.Parameters.AddWithValue("@ManagerId", 1); // ManagerId is Employee 1
                command.Parameters.AddWithValue("@Enabled", true);
                command.ExecuteNonQuery();

                // Employee 5: Manager is Employee 4
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@Name", "Employee 5");
                command.Parameters.AddWithValue("@ManagerId", 4); // ManagerId is Employee 4
                command.Parameters.AddWithValue("@Enabled", true);
                command.ExecuteNonQuery();

                // Employee 6: No manager
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@Name", "Employee 6");
                command.Parameters.AddWithValue("@ManagerId", DBNull.Value);
                command.Parameters.AddWithValue("@Enabled", true);
                command.ExecuteNonQuery();

                // Employee 7: Manager is Employee 6
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@Name", "Employee 7");
                command.Parameters.AddWithValue("@ManagerId", 6); // ManagerId is Employee 6
                command.Parameters.AddWithValue("@Enabled", true);
                command.ExecuteNonQuery();

                // Employee 8: No manager
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@Name", "Employee 8");
                command.Parameters.AddWithValue("@ManagerId", DBNull.Value);
                command.Parameters.AddWithValue("@Enabled", false);
                command.ExecuteNonQuery();

                // Employee 9: Manager is Employee 8
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@Name", "Employee 9");
                command.Parameters.AddWithValue("@ManagerId", 8); // ManagerId is Employee 8
                command.Parameters.AddWithValue("@Enabled", true);
                command.ExecuteNonQuery();

                // Employee 10: No manager
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@Name", "Employee 10");
                command.Parameters.AddWithValue("@ManagerId", DBNull.Value);
                command.Parameters.AddWithValue("@Enabled", true);
                command.ExecuteNonQuery();
            }
        }
    }

    public static void SeedManagerData(string connectionString, string databaseName)
    {
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();

            connection.ChangeDatabase(databaseName);


            var selectCommandText = "SELECT COUNT(*) FROM Manager";
            using (var command = new SqlCommand(selectCommandText, connection))
            {
                int employeeCount = Convert.ToInt32(command.ExecuteScalar());
                if (employeeCount > 5)
                {
                    return;
                }
            }

            var insertCommandText = "INSERT INTO Manager (EmployeeId) VALUES (@EmployeeId)";

            using (var command = new SqlCommand(insertCommandText, connection))
            {
                command.Parameters.AddWithValue("@EmployeeId", 2); 
                command.ExecuteNonQuery();

                command.Parameters.Clear();
                command.Parameters.AddWithValue("@EmployeeId", 3); 
                command.ExecuteNonQuery();

                command.Parameters.Clear();
                command.Parameters.AddWithValue("@EmployeeId", 4); 
                command.ExecuteNonQuery();

                command.Parameters.Clear();
                command.Parameters.AddWithValue("@EmployeeId", 5); 
                command.ExecuteNonQuery();

                command.Parameters.Clear();
                command.Parameters.AddWithValue("@EmployeeId", 7); 
                command.ExecuteNonQuery();

                command.Parameters.Clear();
                command.Parameters.AddWithValue("@EmployeeId", 9); 
                command.ExecuteNonQuery();
            }
        }
    }
}
