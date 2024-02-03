using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Practical_Assignment_221519Z.Model;
using Practical_Assignment_221519Z.ViewModels;
using System.Net;
using System.Text.Json;


namespace Practical_Assignment_221519Z.Pages
{
    public class RegisterModel : PageModel
    {
        private UserManager<ApplicationUser> userManager { get; }
        private SignInManager<ApplicationUser> signInManager { get; }
        private AuthDbContext authDbContext { get; }


        [BindProperty]
        public Register RModel { get; set; }

        public RegisterModel(UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        AuthDbContext authDbContext
        )

        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.authDbContext = authDbContext;
        }

        public void OnGet()
        {
        }

        public class MyObject
        {
            public bool success { get; set; }
        }

        public bool ValidateCaptcha()
        {
            string response = Request.Form["g-recaptcha-response"];

            // Request to Google Server for reCAPTCHA validation
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create($"https://www.google.com/recaptcha/api/siteverify?secret=6LdxT2UpAAAAACIel2VbEsaXpLiCBshAziP5Eysv &response={response}");

            try
            {
                using (WebResponse wResponse = req.GetResponse())
                {
                    using (StreamReader readStream = new StreamReader(wResponse.GetResponseStream()))
                    {
                        string jsonResponse = readStream.ReadToEnd();
                        var data = JsonSerializer.Deserialize<MyObject>(jsonResponse);
                        return data.success;
                    }
                }
            }
            catch (WebException ex)
            {
                throw ex;
            }
        }


        private bool HasSqlInjection(string input)
        {
            // Implement SQL injection detection logic
            // Example: Check for common SQL keywords
            string[] sqlKeywords = { "SELECT", "INSERT", "UPDATE", "DELETE", "DROP", "CREATE" };
            return sqlKeywords.Any(keyword => input.ToUpper().Contains(keyword));
        }

		private bool IsValidEmail(string email)
		{
			try
			{
				var addr = new System.Net.Mail.MailAddress(email);
				return addr.Address == email;
			}
			catch
			{
				return false;
			}
		}

		public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                // Encryption
                var dataProtectionProvider = DataProtectionProvider.Create("EncryptData");
                var protector = dataProtectionProvider.CreateProtector("221519Z_AppSec");
                
                var user = new ApplicationUser()
					{
						UserName = WebUtility.HtmlEncode(RModel.Email),
						Email = WebUtility.HtmlEncode(RModel.Email),
						FullName = WebUtility.HtmlEncode(RModel.FullName),
				        CreditCardNo = WebUtility.HtmlEncode(protector.Protect(RModel.CreditCardNo)),
						Gender = WebUtility.HtmlEncode(RModel.Gender),
						MobileNo = WebUtility.HtmlEncode(RModel.MobileNo),
						DeliveryAddress = WebUtility.HtmlEncode(RModel.DeliveryAddress),
						Photo = WebUtility.HtmlEncode(RModel.Photo),
						AboutMe = WebUtility.HtmlEncode(RModel.AboutMe),
					};
                

                if (HasSqlInjection(RModel.Email))
                {
                    ModelState.AddModelError("", "Invalid characters in the input.");
                   return Page();
                }
                if (!IsValidEmail(RModel.Email))
                {
                    ModelState.AddModelError("", "Invalid email format.");
                    return Page();
                }
                var result = await userManager.CreateAsync(user, RModel.Password);
                if (result.Succeeded)
					{
                    authDbContext.AddToAuditLog(user.UserName, "User Registered");
                    return RedirectToPage("Login");
					}
					foreach (var error in result.Errors)
					{
						ModelState.AddModelError("", error.Description);
					}
                    var isCaptchaValid = ValidateCaptcha();
                    if (!isCaptchaValid)
                    {
                        ModelState.AddModelError("", "reCAPTCHA validation failed. Please try again.");
                        return Page();
                    }


            }
            return Page();
        }
    }
}
