using Admin.Extensions;
using Admin.Services.Endpoints;
using Admin.Services.Interfaces;
using Common.Requests.Employees;
using Common.Responses.Employees;
using Common.Responses.Wrappers;
using System.Net.Http.Json;

namespace Admin.Services.Implementations
{
    public class EmployeeService : IEmployeeService
    {
        private readonly HttpClient _httpClient;
        private readonly ApiEndpoints _apiEndpoints;

        public EmployeeService(HttpClient httpClient, ApiEndpoints apiEndpoints)
        {
            _httpClient = httpClient;
            _apiEndpoints = apiEndpoints;
        }

        public async Task<IResponseWrapper<EmployeeResponse>> CreateAsync(CreateEmployeeRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync(_apiEndpoints.EmployeeEndpoints.Create, request);
            return await response.WrapResponse<EmployeeResponse>();
        }

        public async Task<IResponseWrapper> DeleteAsync(int employeeId)
        {
            var response = await _httpClient.DeleteAsync($"{_apiEndpoints.EmployeeEndpoints.Delete}{employeeId}");
            return await response.WrapResponse<EmployeeResponse>();
        }

        public async Task<IResponseWrapper<List<EmployeeResponse>>> GetAllAsync()
        {
            var response = await _httpClient.GetAsync(_apiEndpoints.EmployeeEndpoints.GetAll);
            return await response.WrapResponse<List<EmployeeResponse>>();
        }

        public async Task<IResponseWrapper<EmployeeResponse>> GetByIdAsync(int employeeId)
        {
            var response = await _httpClient.GetAsync($"{_apiEndpoints.EmployeeEndpoints.GetById}{employeeId}");
            return await response.WrapResponse<EmployeeResponse>();
        }

        public async Task<IResponseWrapper<EmployeeResponse>> UpdateAsync(UpdateEmployeeRequest request)
        {
            var response = await _httpClient.PutAsJsonAsync(_apiEndpoints.EmployeeEndpoints.Update, request);
            return await response.WrapResponse<EmployeeResponse>();
        }
    }
}
