using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Practical_Assignment_221519Z.Model;

namespace Practical_Assignment_221519Z.Pages
{
    public class IndexModel : PageModel
    {
        private UserManager<ApplicationUser> userManager { get; }
        private SignInManager<ApplicationUser> signInManager { get; }
        public ApplicationUser CurrentUser { get; set; }

        public string CreditCardNo { get; set; }

        public IndexModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        public async Task ClearSession()
        {
            await signInManager.SignOutAsync();
            HttpContext.Session.Clear();
            var cookies = Request.Cookies.Keys;
            foreach (var cookie in cookies)
            {
                Response.Cookies.Delete(cookie);
            }

            if (Request.Cookies["ASP.NET_SessionId"] != null)
            {
                Response.Cookies.Append("ASP.NET_SessionId", string.Empty, new CookieOptions
                {
                    Expires = DateTime.Now.AddMonths(-20)
                });
            }

            // Clear AuthToken cookie
            if (Request.Cookies["AuthToken"] != null)
            {
                Response.Cookies.Append("AuthToken", string.Empty, new CookieOptions
                {
                    Expires = DateTime.Now.AddMonths(-20)
                });
            }
        }


        public async Task<IActionResult> OnGet()
        {
            var existingAuthToken = HttpContext.Session.GetString("AuthToken");
            if (existingAuthToken != Request.Cookies["AuthToken"])
            {
                // Session is not valid, redirect to the login page
                await ClearSession();
                return RedirectToPage("Login");
            }
            else
            {
                if (signInManager.IsSignedIn(User))
                {
                    CurrentUser = await userManager.GetUserAsync(User);
                    var dataProtectionProvider = DataProtectionProvider.Create("EncryptData");
                    var protector = dataProtectionProvider.CreateProtector("221519Z_AppSec");
                    CreditCardNo = protector.Unprotect(CurrentUser.CreditCardNo);

                    // Continue with the execution of the page
                    return Page();
                }
            }

            // Session is not valid, redirect to the login page
            await ClearSession();
            return RedirectToPage("Login");
        }



    }
}
