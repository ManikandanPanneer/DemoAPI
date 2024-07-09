using DemoAPI.ApplicationModel;
using DemoAPI.ApplicationModel.DTO;
using DemoAPI.ApplicationModel.Services.AuthenticationRepository.Interface;
using DemoAPI.ApplicationModel.Services.Interface;
using DemoAPI.DataModel.AuthenticationModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;

namespace DemoAPI.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {

        private readonly IAuthenticationUser _Service;
       

        private APIResponse _response;
        public AuthenticationController(IAuthenticationUser Service)
        {
            _response = new APIResponse();
            _Service = Service;
        }
       
        [HttpPost]
        [Route("Register")]
        public async Task<ActionResult<APIResponse>> Register(UserRegisterDto user)
        {
            try
            {
                var id = await _Service.RegisterUser(user);
                if(id == 0)
                {
                    _response.statusCode = System.Net.HttpStatusCode.OK;
                    _response.IsSuccess = false;
                    _response.Message = CommonMessage.UserExist;
                    return _response;
                }
                _response.statusCode = System.Net.HttpStatusCode.OK;
                _response.IsSuccess = true;
                _response.Message = CommonMessage.SuccessRegistration;
                _response.result = id;

            }
            catch (Exception e)
            {

                _response.statusCode = System.Net.HttpStatusCode.InternalServerError;
                _response.IsSuccess = false;
                _response.Message = CommonMessage.FailedRegistration;
                _response.ErrorMessage = e.Message;
            }
            
            return _response;
        }

        [HttpPost]
        [Route("Login")]
        public async Task<ActionResult<APIResponse>> Login(LoginDto login)
        {         

            try
            {
                var user = await _Service.ValidateUser(login);
                if(user == null)
                {
                    _response.statusCode = System.Net.HttpStatusCode.NotFound;
                    _response.IsSuccess = false;
                    _response.Message = CommonMessage.FailedAuth;
                    return _response;
                }
                _response.statusCode = System.Net.HttpStatusCode.OK;
                _response.IsSuccess = true;
                _response.Message = CommonMessage.SuccessAuth;
                _response.result = await _Service.GenerateTocken(user);

            }
            catch (Exception e)
            {

                _response.statusCode = System.Net.HttpStatusCode.InternalServerError;
                _response.IsSuccess = false;
                _response.Message = CommonMessage.FailedAuth;
                _response.ErrorMessage = e.Message;
            }

            return _response;
        }

       
    }
}
