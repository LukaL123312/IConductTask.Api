using IConductTestTask.Application.Employee.Commands;
using IConductTestTask.Application.Employee.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IConductTestTask.Api.Controllers;

[Route("api/employees")]
public class EmployeeController : ApiControllerBase
{
    [HttpGet("GetEmployeeById")]
    [AllowAnonymous]
    public async Task<IActionResult> GetEmployeeById([FromQuery] GetEmployeeByIdQuery query, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPut("EnableEmployee")]
    [AllowAnonymous]
    public async Task<IActionResult> EnableEmployee([FromBody] EnableEmployeeByIdCommand command, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);
        return Ok(result);
    }
}

