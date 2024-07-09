using DemoAPI.ApplicationModel.DTO;
using DemoAPI.DataModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoAPI.ApplicationModel.Services.Interface
{
    public interface IEmployeeService
    {
        Task<List<Employee>> GetAllEmployees();
     
        Task<int> AddEmployee(CreateEmployeeDTO createEmployeeDto);
        Task<int> UpdateEmployee(UpdateEmployeeDto employee);
        Task<int> DeleteEmployee(int id);
    }
}
