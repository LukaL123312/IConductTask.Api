using MediatR;

namespace IConductTestTask.Application.Employee.Commands;

public class EnableEmployeeByIdCommand : IRequest
{
    public int EmployeeId { get; set; }
}