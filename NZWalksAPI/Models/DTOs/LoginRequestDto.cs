using System.ComponentModel.DataAnnotations;

namespace NZWalks.API.Models.DTOs
{
    public class LoginRequestDto
    {
        [Required(ErrorMessage = "Username or EmailAddress is required.")]
        public string LoginId { get; set; }

        [Required(ErrorMessage ="Password is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
