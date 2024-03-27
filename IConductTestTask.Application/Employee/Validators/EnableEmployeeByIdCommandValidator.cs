using FluentValidation;

namespace IConductTestTask.Application.Employee.Validators;

public class EnableEmployeeByIdCommandValidator : AbstractValidator<Commands.EnableEmployeeByIdCommand>
{
    public EnableEmployeeByIdCommandValidator()
    {
        RuleFor(u => u.EmployeeId)
            .NotEmpty()
            .Must(employeeId => employeeId > 0)
            .WithMessage("EmployeeId can't be negative");
    }
}
