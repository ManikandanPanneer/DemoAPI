using Microsoft.AspNetCore.Http;

namespace DemoAPI.ApplicationModel.DTO
{
    public class UpdateEmployeeDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime DOB { get; set; }
        public int Age { get; set; }
        public string Designation { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Gender { get; set; }
        public bool IsActive { get; set; }
        public IFormFile? Image { get; set; }
    }
}
