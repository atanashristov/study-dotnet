using Common.Requests.Employees;
using Common.Responses.Employees;
using Common.Responses.Wrappers;

namespace Admin.Services.Interfaces
{
    public interface IEmployeeService : ITransient
    {
        Task<IResponseWrapper<EmployeeResponse>> CreateAsync(CreateEmployeeRequest request);
        Task<IResponseWrapper<EmployeeResponse>> UpdateAsync(UpdateEmployeeRequest request);
        Task<IResponseWrapper> DeleteAsync(int employeeId);
        Task<IResponseWrapper<List<EmployeeResponse>>> GetAllAsync();
        Task<IResponseWrapper<EmployeeResponse>> GetByIdAsync(int employeeId);
    }
}
