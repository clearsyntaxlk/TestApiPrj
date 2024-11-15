using Microsoft.AspNetCore.Mvc;
using TestApi.Type;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using Test.Models.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
// for view the exsample visit https://medium.com/@vndpal/how-to-implement-jwt-token-authentication-in-net-core-6-ab7f48470f5c

namespace TestApi.Controllers
{

    [Authorize]   // this is for JWT token authorization 
    //[Authorize(Roles =UserRoles.Admin)]   // only can accedd admin userroles , this is for JWT token authorization 
    [Route("mytest/2/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IConfiguration _config;
        public EmployeeController(IConfiguration config) { 
            this._config = config;
        }

        // GET: api/<LogIn>
        [Authorize(Roles ="admin")]
        [HttpGet]
        public async Task<UserDto> Get()
        {
            var _senderMailid = _config.GetSection("MailSetting")["senferMailId"];


            var ob= new UserDto();
            
            ob.users= (new List<User> { new User { Name = "Chaminda",Age=20,MdfOn=DateTime.Now }, new User { Name = "Sarath" ,Age = 22,MdfOn=new DateTime(2024,7,8),EmailId=_senderMailid } });
            ob.status = "OK";
            return ob;
            
            //return (new List<User> { new User { Name = "Chaminda" } });
            //var result = JsonConvert.SerializeObject(new List<User> { new User { Name = "Chaminda" } });
            //return result .ToString();
            //return (new string[] { "Chaminda 1", "Cahminda 2" }).AsEnumerable();
        }

    // GET api/<LogIn>/5
    [HttpGet("{id}")]
        public string Get(int id)
        {
            return "Cahminda ";
        }

        // POST api/<LogIn>
        [HttpPost]
        public void Post([FromBody] string value)
        {

        }

        // PUT api/<LogIn>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<LogIn>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
