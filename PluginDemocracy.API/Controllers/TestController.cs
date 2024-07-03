using Microsoft.AspNetCore.Mvc;
using PluginDemocracy.DTOs;
using PluginDemocracy.API.UrlRegistry;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PluginDemocracy.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        // GET: api/<UtilityController>
        [HttpGet("testmessagespage")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<PDAPIResponse> TestMessagesPageWithRedirection()
        {
            PDAPIResponse apiResponse = new()
            {
                RedirectTo = FrontEndPages.GenericMessage,
            };
            //Fill generic message dictionary
            apiResponse.RedirectParameters["Title"] = "Testing Title";
            apiResponse.RedirectParameters["Body"] = "Testing body lorem ipsum lorem ipsum lorem";
            //Fill Alert messages
            apiResponse.AddAlert("info", "Testing info message");
            apiResponse.AddAlert("warning", "Testing warning message");
            apiResponse.AddAlert("success", "Testing success message");
            apiResponse.AddAlert("normal", "Testing normal message");
            apiResponse.AddAlert("Error", "Testing error message");

            //return
            return apiResponse;
        }
    }
}
