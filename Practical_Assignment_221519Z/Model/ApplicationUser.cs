using Microsoft.AspNetCore.Identity;

namespace Practical_Assignment_221519Z.Model
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }

        public string CreditCardNo { get; set; }

        public string Gender { get; set; }

        public string MobileNo { get; set; }

        public string DeliveryAddress { get; set; }

        public string Photo { get; set; }

        public string AboutMe { get; set; }

    }
}
