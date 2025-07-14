using DeductionsPractice.Lib;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DeductionsPractice.Web.Pages
{
    public class LoginModel : PageModel
    {
        private readonly ILogger<LoginModel> _logger;
        private readonly NdasendaApiClient _apiClient;

        public LoginModel(ILogger<LoginModel> logger, NdasendaApiClient apiClient)
        {
            _logger = logger;
            _apiClient = apiClient;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            // Set temporary credentials
            _apiClient.OverrideCredentials(Input.Email, Input.Password);

            var isAuthenticated = await _apiClient.EnsureAuthenticatedAsync();
            if (!isAuthenticated)
            {
                ErrorMessage = "Login failed. Check your credentials.";
                return Page();
            }

            var account = await _apiClient.CheckAccountAsync(new Security
            {
                SecurityToken = _apiClient.SecurityToken // already in config
            });

            if (account == null)
            {
                ErrorMessage = "Failed to fetch user profile.";
                return Page();
            }

            // Setup claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, Input.Email),
                new Claim("AccessToken", _apiClient.AccessToken ?? ""),
                new Claim(ClaimTypes.Role, account.UserAccountRole.ToString())
            };

            foreach (var code in account.DeductionCodes ?? new())
            {
                claims.Add(new Claim("DeductionCode", code.Code));
            }

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            // Route by role
            return RedirectToPage(account.UserAccountRole.ToString() switch
            {
                "ORGANIZATION_ADMIN" => "/Admin/Dashboard",
                "ORGANIZATION_USER" => "/User/Dashboard",
                _ => "/Index"
            });
        }
    }
}
