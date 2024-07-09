using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DemoAPI.DataModel.AuthenticationModel;
using Newtonsoft.Json.Linq;

namespace DemoAPI.DataModel.DataAccessLayer
{
    public class AuthenticationContext
    {
        private readonly string _connectionString;

        public AuthenticationContext(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("LocalConnect");
        }

        public async Task<User> ValidateUser(string email, string password)
        {
            User user = null;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_ValidateUser", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Password", password);
                    conn.Open();
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();
                    if (reader.Read())
                    {
                        user = new User
                        {
                            UserId = (int)reader["UserId"],
                            Email = (string)reader["Email"],
                            AccessId = (string)reader["AccessId"]
                        };
                    }
                    conn.Close();
                }
            }

            return user;
        }

        public async Task<int> Register(User user)
        {
            int value = 0;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_RegisterUser", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Email", user.Email);
                    cmd.Parameters.AddWithValue("@Password", user.Password);
                    cmd.Parameters.AddWithValue("@AccessId", user.AccessId);
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
