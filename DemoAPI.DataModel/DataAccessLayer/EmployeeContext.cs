using DemoAPI.DataModel.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Data;
using System.Data.SqlClient;

namespace DemoAPI.DataModel.DataAccessLayer
{
    public class EmployeeContext
    {
        private readonly string _connectionString;


        /// <summary>
        /// Getting the connection string form the appsetting.json and storing in _connectionString
        /// </summary>      
        public EmployeeContext(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("LocalConnect");
        }


        /// <summary>
        /// DAL - used to Get the list of Employee
        /// </summary>
        /// <returns> Employee List</returns>
        public async Task<List<Employee>> GetAllEmployees()
        {
            var employees = new List<Employee>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_GetAllEmployees", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;                   
                    
                    DataTable dt = new DataTable();
                    conn.Open();
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                    dataAdapter.Fill(dt);
                    conn.Close();
                    employees = JsonConvert.DeserializeObject<List<Employee>>(JsonConvert.SerializeObject(dt));                   
                }
            }
            return employees;
        }


        /// <summary>
        /// DAL - Used to Add new employee in DB
        /// </summary>
        /// <param name="employee"> Employee Details</param>
        /// <returns> New Employee ID</returns>
        public async Task<int> AddEmployee(Employee employee)
        {
            int value = 0;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_AddEmployee", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Name", employee.Name);
                    cmd.Parameters.AddWithValue("@DOB", employee.DOB.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@Age", employee.Age);
                    cmd.Parameters.AddWithValue("@Designation", employee.Designation);
                    cmd.Parameters.AddWithValue("@Email", employee.Email);
                    cmd.Parameters.AddWithValue("@Gender", employee.Gender);
                    cmd.Parameters.AddWithValue("@Phone", employee.Phone);
                    if(employee.IsActive)
                    {
                        cmd.Parameters.AddWithValue("@IsActive",1);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@IsActive", 0);
                    }
                    
                    cmd.Parameters.AddWithValue("@Image", employee.Image);
                    conn.Open();
                    var Id = await cmd.ExecuteScalarAsync();
                    conn.Close();
                    if (Id != null)
                    {
                        value = Convert.ToInt32(Id);
                    }
                }
            }
            return value;
        }


        /// <summary>
        /// DAL - To update Existing Employee
        /// </summary>
        /// <param name="employee">Employee Details</param>
        /// <returns>Employee Details</returns>
        public async Task<int> UpdateEmployee(Employee employee)
        {
            int value = 0;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_UpdateEmployee", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", employee.Id);
                    cmd.Parameters.AddWithValue("@Name", employee.Name);
                    cmd.Parameters.AddWithValue("@DOB", employee.DOB.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@Age", employee.Age);
                    cmd.Parameters.AddWithValue("@Designation", employee.Designation);
                    cmd.Parameters.AddWithValue("@Email", employee.Email);
                    cmd.Parameters.AddWithValue("@Gender", employee.Gender);
                    cmd.Parameters.AddWithValue("@Phone", employee.Phone);
                    if (employee.IsActive)
                    {
                        cmd.Parameters.AddWithValue("@IsActive", 1);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@IsActive", 0);
                    }

                    cmd.Parameters.AddWithValue("@Image", employee.Image);
                    conn.Open();
                    var Id = await cmd.ExecuteScalarAsync(); 
                    conn.Close();
                    if(Id != null)
                    {
                        value = Convert.ToInt32(Id);
                    }                    
                }
            }
            return value;
        }

        /// <summary>
        /// DAL To delete Particular  Employee 
        /// </summary>
        /// <param name="id">Employee ID</param>
        /// <returns>Employee Id</returns>
        public async Task<int> DeleteEmployee(int id)
        {
            int value = 0;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_DeleteEmployee", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", id);
                    conn.Open();
                    var Id = await cmd.ExecuteScalarAsync();
                    conn.Close();
                    if (Id != null)
                    {
                        value = Convert.ToInt32(Id);
                    }
                }
            }
            return value;
        }
    }
}
