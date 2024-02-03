using System.ComponentModel.DataAnnotations;

namespace Practical_Assignment_221519Z.ViewModels
{
	public class Login
	{
		[Required]
		[DataType(DataType.EmailAddress, ErrorMessage = "Invalid email format.")]
		public string Email { get; set; }
		
		[Required]
		[DataType(DataType.Password)]
		public string Password { get; set; }
		public bool RememberMe { get; set; }
    }
}
