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

        public EmployeeContext(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("LocalConnect");
        }

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

        public async Task<int> AddEmployee(Employee employee)
        {
            int value = 0;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_AddEmployee", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Name", employee.Name);
                    cmd.Parameters.AddWithValue("@Email", employee.Email);
                    cmd.Parameters.AddWithValue("@Phone", employee.Phone);
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
                    cmd.Parameters.AddWithValue("@Email", employee.Email);
                    cmd.Parameters.AddWithValue("@Phone", employee.Phone);
                    cmd.Parameters.AddWithValue("@Image", employee.Image ?? (object)DBNull.Value);
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
