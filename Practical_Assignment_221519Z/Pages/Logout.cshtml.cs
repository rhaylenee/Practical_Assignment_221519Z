using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Practical_Assignment_221519Z.Model;
using System.Net;

namespace Practical_Assignment_221519Z.Pages
{
    public class LogoutModel : PageModel
    {
		private readonly SignInManager<ApplicationUser> signInManager;
        private AuthDbContext authDbContext { get; }
        public LogoutModel(SignInManager<ApplicationUser> signInManager, AuthDbContext authDbContext)

		{
			this.signInManager = signInManager;
            this.authDbContext = authDbContext;
        }
		public void OnGet()
		{
		}

		public async Task<IActionResult> OnPostLogoutAsync()
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

            authDbContext.AddToAuditLog(User.Identity.Name, "User Logged Out");
            return RedirectToPage("Login");
		}
		public async Task<IActionResult> OnPostDontLogoutAsync()
		{
			return RedirectToPage("Index");
		}
	}
}
