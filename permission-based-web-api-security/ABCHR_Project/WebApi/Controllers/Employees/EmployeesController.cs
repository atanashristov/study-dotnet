using Application.Features.Employees.Commands;
using Application.Features.Employees.Queries;
using Common.Authorization;
using Common.Requests.Employees;
using Common.Responses.Employees;
using Common.Responses.Wrappers;
using Microsoft.AspNetCore.Mvc;
using WebApi.Attributes;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeesController : BaseController<EmployeesController>
    {
        [HttpPost]
        [MustHavePermission(AppFeature.Employees, AppAction.Create)]
        public async Task<ActionResult<IResponseWrapper<EmployeeResponse>>> CreateEmployee([FromBody] CreateEmployeeRequest createEmployee)
        {
            var response = await MediatorSender
                .Send(new CreateEmployeeCommand { CreateEmployeeRequest = createEmployee });
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPut]
        [MustHavePermission(AppFeature.Employees, AppAction.Update)]
        public async Task<ActionResult<IResponseWrapper<EmployeeResponse>>> UpdateEmployee([FromBody] UpdateEmployeeRequest updateEmployee)
        {
            var response = await MediatorSender
                .Send(new UpdateEmployeeCommand { UpdateEmployeeRequest = updateEmployee });
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpDelete("{employeeId}")]
        [MustHavePermission(AppFeature.Employees, AppAction.Delete)]
        public async Task<ActionResult<IResponseWrapper<int>>> DeleteEmployee(int employeeId)
        {
            var response = await MediatorSender.Send(new DeleteEmployeeCommand { EmployeeId = employeeId });
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return NotFound(response);
        }

        [HttpGet]
        [MustHavePermission(AppFeature.Employees, AppAction.Read)]
        public async Task<ActionResult<IResponseWrapper<IEnumerable<EmployeeResponse>>>> GetEmployeeList()
        {
            var response = await MediatorSender.Send(new GetEmployeesQuery());
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return NotFound(response);
        }

        [HttpGet("{employeeId}")]
        [MustHavePermission(AppFeature.Employees, AppAction.Read)]
        public async Task<ActionResult<IResponseWrapper<EmployeeResponse>>> GetEmployeeById(int employeeId)
        {
            var response = await MediatorSender.Send(new GetEmployeeByIdQuery { EmployeeId = employeeId });
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return NotFound(response);
        }
    }
}
