using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bearer_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult<string> Get()
        {
            return "Hello from my bearer-protected 3rdparty .net core service - only with role ADMIN! :-)";
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "user")]
        public ActionResult<string> GetById(int id)
        {
            return "Hello from my bearer-protected 3rdparty .net core service - only with role USER! :-)";
        }

        
        
        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}