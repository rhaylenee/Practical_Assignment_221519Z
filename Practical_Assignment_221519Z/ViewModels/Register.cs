using System.ComponentModel.DataAnnotations;

namespace Practical_Assignment_221519Z.ViewModels
{
    public class Register
    {
        [Required]
		[DataType(DataType.EmailAddress, ErrorMessage = "Invalid email format.")]
		public string Email { get; set; }

        [Required]
        // [DataType(DataType.Password)]
        [MinLength(12, ErrorMessage = "Password Length must be at least 12 characters")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[$@$!%*?&])[A-Za-z\d$@$!%*?&]{8,}$",
        ErrorMessage = "Passwords must be at least 12 characters long and contain at least an uppercase letter, lower case letter, digit and a special character")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Password and confirmation password does not match")]
        public string ConfirmPassword { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string FullName { get; set; }


        [Required] // must be encrypted
        [DataType(DataType.CreditCard)]
        public string CreditCardNo { get; set; }


        [Required]
        [DataType(DataType.Text)]
        public string Gender { get; set; }

        [Required]
        [DataType(DataType.PhoneNumber)]
        public string MobileNo { get; set; }


        [Required]
        [DataType(DataType.Text)]
        public string DeliveryAddress { get; set; }



        [Required(ErrorMessage = "Please upload a JPG image.")]
        [RegularExpression(@"^.+\.jpg$", ErrorMessage = "Only JPG files are allowed.")]
        public string Photo { get; set; }


        [Required]
        [RegularExpression(@"^[^\r\n]+$", ErrorMessage = "Special characters are allowed.")]
        public string AboutMe { get; set; }

        
    }
}
