using Application.Features.Employees.Commands;
using Application.Services;
using FluentValidation;

namespace Application.Features.Employees.Validators
{
    public class UpdateEmployeeCommandValidator : AbstractValidator<UpdateEmployeeCommand>
    {
        public UpdateEmployeeCommandValidator(IEmployeeService employeeService)
        {
            RuleFor(command => command.UpdateEmployeeRequest)
                .SetValidator(new UpdateEmployeeRequestValidator(employeeService));
        }
    }
}
