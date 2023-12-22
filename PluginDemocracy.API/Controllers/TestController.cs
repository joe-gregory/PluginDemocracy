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
            PDAPIResponse apiResponse = new();
            apiResponse.RedirectTo = FrontEndPages.GenericMessage;
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
        // GET: api/Utility/values
        [HttpGet("values")]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<UtilityController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<UtilityController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<UtilityController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<UtilityController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
