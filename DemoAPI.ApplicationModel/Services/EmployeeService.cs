using DemoAPI.ApplicationModel.DTO;
using DemoAPI.ApplicationModel.Services.Interface;
using DemoAPI.DataModel.DataAccessLayer;
using DemoAPI.DataModel.Models;

namespace DemoAPI.ApplicationModel.Services
{
    /// <summary>
    /// Consist All the services of Employee Calling from Employee controller
    /// Inherits from IEmployeeService
    /// </summary>
    public class EmployeeService : IEmployeeService
    {
        private readonly EmployeeContext _employeeContext;
        public EmployeeService(EmployeeContext employeeContext)
        {
            _employeeContext = employeeContext;
        }

        public async Task<int> AddEmployee(CreateEmployeeDTO employeeDto)
        {
            var employee = new Employee
            {
                Name = employeeDto.Name,
                DOB= employeeDto.DOB,
                Age= employeeDto.Age,
                Designation= employeeDto.Designation,
                Email = employeeDto.Email,
                Phone = employeeDto.Phone,
                Gender= employeeDto.Gender,
                IsActive= employeeDto.IsActive
            };

            if (employeeDto.Image != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await employeeDto.Image.CopyToAsync(memoryStream);
                    employee.Image = memoryStream.ToArray();
                }
            }

            int cou = await _employeeContext.AddEmployee(employee);
            return cou;
            
        }

        public async Task<int> UpdateEmployee(UpdateEmployeeDto employeeDto)
        {
            var employee = new Employee
            {
                Id = employeeDto.Id,
                Name = employeeDto.Name,
                DOB = employeeDto.DOB,
                Age = employeeDto.Age,
                Designation = employeeDto.Designation,
                Email = employeeDto.Email,
                Phone = employeeDto.Phone,
                Gender = employeeDto.Gender,
                IsActive = employeeDto.IsActive
            };

            if (employeeDto.Image != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await employeeDto.Image.CopyToAsync(memoryStream);
                    employee.Image = memoryStream.ToArray();
                }
            }

            int cou = await _employeeContext.UpdateEmployee(employee);
            return cou;
        }

        public async Task<int> DeleteEmployee(int id)
        {           
            return await _employeeContext.DeleteEmployee(id);
        }

        public async Task<List<Employee>> GetAllEmployees()
        {           
            return await _employeeContext.GetAllEmployees();
        }

       
    }
}
