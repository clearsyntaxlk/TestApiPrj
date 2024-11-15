using System.ComponentModel.DataAnnotations;
using TestApi.Enums;

namespace TestApi.Type.BodyRequest
{
    public class LogInRequest
    {
        [Required(ErrorMessage = "User Name is required")]
        public string UserName { get; set; }
        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string? Email { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
        public GrantType GrantType { get; set; }  // thare are two types 1: password, 2: refresh_token

    }
}
