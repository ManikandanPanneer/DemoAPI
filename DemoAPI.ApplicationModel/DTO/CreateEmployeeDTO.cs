using Microsoft.AspNetCore.Http;

namespace DemoAPI.ApplicationModel.DTO
{
    public class CreateEmployeeDTO
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public IFormFile? Image { get; set; }
    }
}
