using Microsoft.AspNetCore.Http;

namespace DemoAPI.ApplicationModel.DTO
{
    public class UpdateEmployeeDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public IFormFile Image { get; set; }
    }
}
