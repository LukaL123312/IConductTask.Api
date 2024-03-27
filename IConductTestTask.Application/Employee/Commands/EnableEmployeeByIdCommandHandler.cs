using IConductTestTask.Application.Common.Exceptions;
using IConductTestTask.Application.Common.Interfaces.Repositories.Employee;
using MediatR;

namespace IConductTestTask.Application.Employee.Commands;

public class EnableEmployeeByIdCommandHandler : IRequestHandler<EnableEmployeeByIdCommand, Unit>
{
    private readonly IEmployeeRepository _employeeRepository;

    public EnableEmployeeByIdCommandHandler(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(employeeRepository));
    }

    public async Task<Unit> Handle(EnableEmployeeByIdCommand request, CancellationToken cancellationToken)
    {
        await _employeeRepository.EnableEmployee(request.EmployeeId);

        return Unit.Value;
    }
}
