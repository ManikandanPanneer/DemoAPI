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

        /// <summary>
        /// Used to Get The List of Employees
        /// </summary>
        /// <returns> APIResponse.result = Employee List</returns>

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

        /// <summary>
        /// Get the Particular Employee (Get the list of employees and filter from that)
        /// </summary>
        /// <param name="id"> Employee ID</param>
        /// <returns>  APIResponse.result = Employee</returns>
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

        /// <summary>
        /// To Delete the Particular employee
        /// </summary>
        /// <param name="id"> Employee ID</param>
        /// <returns> APIResponse.result =  Employee ID</returns>
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

        /// <summary>
        /// To create a New employee
        /// </summary>
        /// <param name="employeeDto"> Employee DTO Consist Employee Details</param>
        /// <returns> APIResponse.result = New Employee ID</returns>
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

        /// <summary>
        /// To update a existing Employee
        /// </summary>
        /// <param name="employeeDto"> Update Employee Details consist Employee's updatation details</param>
        /// <returns> APIResponse.result = Employee ID</returns>
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




        /// <summary>
        /// To get the Employee Details in a PDF format With Photo
        /// </summary>
        /// <param name="EmpId"> Employee ID</param>
        /// <returns>PDF file</returns>

        [HttpGet]
        [Route("pdf")]
        public async Task<IActionResult> GetEmployeePdf(int EmpId)
        {
            try
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
                        Paragraph dob = new Paragraph("DOB  :" + employee.DOB.ToString()).SetFontSize(16).SetFont(timesbold);
                        document.Add(dob);
                        Paragraph age = new Paragraph("Age :" + employee.Age.ToString()).SetFontSize(16).SetFont(timesbold);
                        document.Add(age);
                        Paragraph Designation = new Paragraph("Designation :" + employee.Designation).SetFontSize(16).SetFont(timesbold);
                        document.Add(Designation);

                        Paragraph Email = new Paragraph("Email :" + employee.Email).SetFontSize(16).SetFont(timesbold);
                        document.Add(Email);

                        Paragraph Phone = new Paragraph("Phone :" + employee.Phone).SetFontSize(16).SetFont(timesbold);
                        document.Add(Phone);
                        Paragraph Gender = new Paragraph("Gender :" + employee.Gender).SetFontSize(16).SetFont(timesbold);
                        document.Add(Gender);
                        


                    }
                }
                var fileName = $"Employee_{employee.Name}.pdf";

                return File(ms.ToArray(), "application/pdf", fileName);
            }
            catch (Exception)
            {

                return BadRequest("Error while getting PDF");
            }
            

        }

        /// <summary>
        /// To insert n-Number employee using CSV file
        /// </summary>
        /// <param name="file">CSV file with employee data</param>
        /// <returns> APIResponse.result = Null , APIResponse.IsSuccess = true / false </returns>
        [HttpPost("InsertEmployeeCsv")]
        public async Task<ActionResult<APIResponse>> UploadCsv(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    _response.statusCode = System.Net.HttpStatusCode.BadRequest;
                    _response.Message = CommonMessage.FailedCreated;
                    return _response;
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
                            DOB = record.DOB,
                            Age = record.Age,
                            Designation= record.Designation,
                            Email = record.Email,
                            Phone = record.Phone,
                            Gender = record.Gender,
                            IsActive = record.IsActive
                        });
                    }
                }

                foreach (var employee in employees)
                {
                    var id = await _Service.AddEmployee(employee);
                }

                _response.statusCode = System.Net.HttpStatusCode.OK;
                _response.IsSuccess = true;
                _response.Message = CommonMessage.SuccessCreated;               

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

        /// <summary>
        /// To Download all the employee data in CSV format
        /// </summary>
        /// <returns> Employee Data CSV </returns>
        [HttpGet("EmployeeDataCsv")]
        public async Task<IActionResult> DownloadCsv()
        {
            try
            {
                var employees = await _Service.GetAllEmployees();
                var employeeCsv = new List<EmployeeCSV>();

                foreach (var employee in employees)
                {
                    employeeCsv.Add(new EmployeeCSV
                    {
                        Name = employee.Name,
                        DOB = employee.DOB,
                        Age = employee.Age,
                        Designation= employee.Designation,
                        Email = employee.Email,
                        Gender = employee.Gender,
                        Phone = employee.Phone,
                        IsActive = employee.IsActive
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
            catch (Exception)
            {

                return BadRequest("Error while dowloading CSV");
            }           
        }
    }
}
