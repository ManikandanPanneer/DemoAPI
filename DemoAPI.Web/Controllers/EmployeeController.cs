using CsvHelper;
using CsvHelper.Configuration;
using DemoAPI.ApplicationModel;
using DemoAPI.ApplicationModel.DTO;
using DemoAPI.ApplicationModel.Services.Interface;
using DemoAPI.Web.CSVModel;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout.Element;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Text;

namespace DemoAPI.Web.Controllers
{
    [Authorize]
    [Route("api")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _Service;

        private APIResponse _response;
        public EmployeeController(IEmployeeService Service)
        {
            _response = new APIResponse();
            _Service = Service;
        }

        [HttpGet]
        [Route("Employees")]
        public async Task<ActionResult<APIResponse>> GetAllEmployees()
        {
            try
            {
                var employees = await _Service.GetAllEmployees();
                if (employees == null)
                {
                    _response.statusCode = System.Net.HttpStatusCode.NotFound;
                    return _response;
                }
                _response.statusCode = System.Net.HttpStatusCode.OK;
                _response.IsSuccess = true;
                _response.Message = CommonMessage.SuccessGet;
                _response.result = employees;
            }
            catch (Exception e)
            {

                _response.statusCode = System.Net.HttpStatusCode.InternalServerError;
                _response.IsSuccess = false;
                _response.Message = CommonMessage.FailedGet;
                _response.ErrorMessage = e.Message;
            }
            return _response;
        }

        [HttpGet]
        [Route("Employee")]
        public async Task<ActionResult<APIResponse>> GetEmployeeById(int id)
        {
            try
            {
                var employees = await _Service.GetAllEmployees();
                var employee = employees.Where(e => e.Id == id).FirstOrDefault();
                if (employee == null)
                {
                    _response.statusCode = System.Net.HttpStatusCode.NotFound;
                    return _response;
                }
                _response.statusCode = System.Net.HttpStatusCode.OK;
                _response.IsSuccess = true;
                _response.Message = CommonMessage.SuccessGet;
                _response.result = employee;
            }
            catch (Exception e)
            {
                _response.statusCode = System.Net.HttpStatusCode.InternalServerError;
                _response.IsSuccess = false;
                _response.Message = CommonMessage.FailedGet;
                _response.ErrorMessage = e.Message;
            }
            return _response;
        }


        [HttpDelete]
        [Route("delete")]
        public async Task<APIResponse> DeleteEmployee(int id)
        {

            try
            {
                var employees = await _Service.GetAllEmployees();
                var employee = employees.Where(e => e.Id == id).FirstOrDefault();
                if (employee == null)
                {
                    _response.statusCode = System.Net.HttpStatusCode.NotFound;
                    return _response;
                }
                var count = await _Service.DeleteEmployee(id);

                _response.statusCode = System.Net.HttpStatusCode.OK;
                _response.IsSuccess = true;
                _response.Message = CommonMessage.SuccessDelete;
                _response.result = count;


            }
            catch (Exception e)
            {

                _response.statusCode = System.Net.HttpStatusCode.InternalServerError;
                _response.IsSuccess = false;
                _response.Message = CommonMessage.FailedDelete;
                _response.ErrorMessage = e.Message;
            }    
            return _response;
        }


        [HttpPost]
        [Route("create")]
        public async Task<APIResponse> AddEmployee([FromForm] CreateEmployeeDTO employeeDto)
        {
            try
            {
                var count = await _Service.AddEmployee(employeeDto);
                _response.statusCode = System.Net.HttpStatusCode.OK;
                _response.IsSuccess = true;
                _response.Message = CommonMessage.SuccessCreated;
                _response.result = count;

            }
            catch (Exception e)
            {
                _response.statusCode = System.Net.HttpStatusCode.InternalServerError;
                _response.IsSuccess = false;
                _response.Message = CommonMessage.FailedCreated;
                _response.ErrorMessage = e.Message;
            }
            return _response;
        }

        [HttpPost]
        [Route("update")]
        public async Task<APIResponse> UpdateEmployee([FromForm] UpdateEmployeeDto employeeDto)
        {
            try
            {
                var employees = await _Service.GetAllEmployees();
                var employee = employees.Where(e=> e.Id == employeeDto.Id).FirstOrDefault();
                if (employee == null)
                {
                    _response.statusCode = System.Net.HttpStatusCode.NotFound;
                    return _response;
                }

                var count = await _Service.UpdateEmployee(employeeDto);
                _response.statusCode = System.Net.HttpStatusCode.OK;
                _response.IsSuccess = true;
                _response.Message = CommonMessage.SuccessUpdate;
                _response.result = count;
            }
            catch (Exception e)
            {
                _response.statusCode = System.Net.HttpStatusCode.InternalServerError;
                _response.IsSuccess = false;
                _response.Message = CommonMessage.FailedUpdate;
                _response.ErrorMessage = e.Message;
            }            
            return _response;
        }






        [HttpGet]
        [Route("pdf")]
        public async Task<IActionResult> GetEmployeePdf(int EmpId)
        {
            var employees = await _Service.GetAllEmployees();
            var employee = employees.Where(e => e.Id == EmpId).FirstOrDefault();
            if (employee == null)
            {
                return NotFound();
            }

            var ms = new MemoryStream();

            using (PdfWriter pdfWriter = new PdfWriter(ms))
            {
                // Create a PdfDocument with the PdfWriter
                using (PdfDocument pdfDocument = new PdfDocument(pdfWriter))
                {
                    // Create a Document to add content to
                    iText.Layout.Document document = new iText.Layout.Document(pdfDocument);

                    // Add content to the document
                    document.SetMargins(30, 30, 30, 30);
                    PdfFont timesbold = PdfFontFactory.CreateFont(StandardFonts.TIMES_BOLD);
                    PdfFont times = PdfFontFactory.CreateFont(StandardFonts.TIMES_ROMAN);
                    document.SetFont(times);
                    float lineWidth = 1f;


                    if (employee.Image != null)
                    {
                        var imageData = iText.IO.Image.ImageDataFactory.Create(employee.Image);
                        var image = new Image(imageData);
                        document.Add(image);
                    }

                    Paragraph id = new Paragraph("ID    :" + employee.Id.ToString()).SetFontSize(16).SetFont(timesbold);
                    document.Add(id);
                    Paragraph Name = new Paragraph("Name  :" + employee.Name).SetFontSize(16).SetFont(timesbold);
                    document.Add(Name);
                    Paragraph mail = new Paragraph("Mail  :" + employee.Email).SetFontSize(16).SetFont(timesbold);
                    document.Add(mail);
                    Paragraph Phone = new Paragraph("Phone :" + employee.Phone).SetFontSize(16).SetFont(timesbold);
                    document.Add(Phone);
                }
            }
            var fileName = $"Employee_{employee.Name}.pdf";

            return File(ms.ToArray(), "application/pdf", fileName);

        }


        [HttpPost("InsertEmployeeCsv")]
        public async Task<IActionResult> UploadCsv(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            var employees = new List<CreateEmployeeDTO>();

            using (var stream = new StreamReader(file.OpenReadStream(), Encoding.UTF8))
            using (var csv = new CsvReader(stream, new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = "," }))
            {
                var records = csv.GetRecords<EmployeeCSV>();
                foreach (var record in records)
                {
                    employees.Add(new CreateEmployeeDTO
                    {
                        Name = record.Name,
                        Email = record.Email,
                        Phone = record.Phone
                    });
                }
            }

            foreach (var employee in employees)
            {
              var id = await _Service.AddEmployee(employee);
            }

            return Ok("Employees inserted successfully.");
        }


        [HttpGet("EmployeeDataCsv")]
        public async Task<IActionResult> DownloadCsv()
        {
            var employees = await _Service.GetAllEmployees();
            var employeeCsv = new List<EmployeeCSV>();

            foreach (var employee in employees)
            {
                employeeCsv.Add(new EmployeeCSV
                {                   
                    Name = employee.Name,
                    Email = employee.Email,
                    Phone = employee.Phone
                });
            }

            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8))
            using (var csvWriter = new CsvWriter(streamWriter, new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = "," }))
            {
                csvWriter.WriteRecords(employeeCsv);
                streamWriter.Flush();
                memoryStream.Position = 0;
                var fileName = "EmployeeList.csv";
                return File(memoryStream.ToArray(), "text/csv", fileName);
            }
        }
    }
}
