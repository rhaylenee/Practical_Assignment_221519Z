using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Practical_Assignment_221519Z.Model;
using Practical_Assignment_221519Z.ViewModels;
using System.Net;
using System.Text.Json;

namespace Practical_Assignment_221519Z.Pages
{
    public class LoginModel : PageModel
    {
		[BindProperty]
		public Login LModel { get; set; }

		private readonly SignInManager<ApplicationUser> signInManager;
        private AuthDbContext authDbContext { get; }
        public LoginModel(SignInManager<ApplicationUser> signInManager, AuthDbContext authDbContext)
		{
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

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                if (HasSqlInjection(LModel.Email))
                {
                    ModelState.AddModelError("", "Invalid characters in the input.");
                    return Page();
                }
                if (!IsValidEmail(LModel.Email))
                {
                    ModelState.AddModelError("", "Invalid email format.");
                    return Page();
                }

                var identityResult = await signInManager.PasswordSignInAsync(LModel.Email, LModel.Password,
                    LModel.RememberMe,lockoutOnFailure: true);
                var isUserLoggedIn = authDbContext.IsUserLoggedIn(LModel.Email);

                if (!isUserLoggedIn)
                {
                    if (identityResult.Succeeded)
                    {
                        // Set session and cookie for successful login
                        
                        HttpContext.Session.SetString("UserId", LModel.Email);
                        string guid = Guid.NewGuid().ToString();
                        HttpContext.Session.SetString("AuthToken", guid);
                        Response.Cookies.Append("AuthToken", guid, new CookieOptions
                        {
                            HttpOnly = true,
                            SameSite = SameSiteMode.Strict,

                        });
                        authDbContext.AddToAuditLog(LModel.Email, "User Logged In");

                        return RedirectToPage("Index");
                    }
                    else if (identityResult.IsLockedOut)
                    {
                        ModelState.AddModelError("", "Account locked out. Try again later.");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Invalid login attempt.");
                    }

                    // reCAPTCHA validation
                    var isCaptchaValid = ValidateCaptcha();
                    if (!isCaptchaValid)
                    {
                        ModelState.AddModelError("", "reCAPTCHA validation failed. Please try again.");
                        return Page();
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Account is logged in on another device.");
                }

                
            }
            return Page();
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
        private bool HasSqlInjection(string input)
        {
            // Implement SQL injection detection logic
            // Example: Check for common SQL keywords
            string[] sqlKeywords = { "SELECT", "INSERT", "UPDATE", "DELETE", "DROP", "CREATE" };
            return sqlKeywords.Any(keyword => input.ToUpper().Contains(keyword));
        }
    }
}
