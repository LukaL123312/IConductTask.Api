using IConductTestTask.Domain;

namespace IConductTestTask.Application.Common.Interfaces.Repositories.Employee;

public interface IEmployeeRepository
{
    Task<Domain.Employee?> GetById(int id);
    Task EnableEmployee(int employeeId);

    Task<Domain.Employee?> GetFullEmployeeTree(int employeeId);
}
