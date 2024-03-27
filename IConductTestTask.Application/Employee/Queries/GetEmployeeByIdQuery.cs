using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace IConductTestTask.Application.Employee.Queries;

public class GetEmployeeByIdQuery : IRequest<Domain.Employee?>
{
    [FromQuery(Name = "employeeiD")]
    public int EmployeeId { get; set; }
}
