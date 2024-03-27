using IConductTestTask.Application.Common.Interfaces.Repositories.Employee;
using MediatR;

namespace IConductTestTask.Application.Employee.Queries;

internal class GetEmployeeByIdQueryHandler
{
}
public class GetEmployeeByIdQueryHandlerHandler : IRequestHandler<GetEmployeeByIdQuery, Domain.Employee?>
{
    private readonly IEmployeeRepository _employeeRepository;

    public GetEmployeeByIdQueryHandlerHandler(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(employeeRepository));
    }

    public async Task<Domain.Employee?> Handle(GetEmployeeByIdQuery request, CancellationToken cancellationToken)
    {
        var response = await _employeeRepository.GetFullEmployeeTree(request.EmployeeId);

        if (response is null)
        {
            return null;
        }

        return response;
    }
}