using Test.Models.Models;

namespace TestApi.Type
{
    public class UserDto : GenericResponse
    {
        public IEnumerable<User> users { get; set; }    
    }
}
