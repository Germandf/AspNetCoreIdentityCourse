using AspNetCoreIdentityCourse.IdentityApp.Data.Account;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentityCourse.IdentityApp.Pages.Account;

public class LoginModel : PageModel
{
    private readonly SignInManager<User> _signInManager;

    public LoginModel(SignInManager<User> signInManager)
    {
        _signInManager = signInManager;
    }

    [BindProperty]
    public CredentialVm Credential { get; set; } = new();

    [BindProperty]
    public IEnumerable<AuthenticationScheme> ExternalLoginProviders { get; set; } = new List<AuthenticationScheme>();

    public async Task OnGetAsync()
    {
        ExternalLoginProviders = await _signInManager.GetExternalAuthenticationSchemesAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var result = await _signInManager.PasswordSignInAsync(
            Credential.Email, Credential.Password, Credential.RememberMe, false);

        if (result.Succeeded)
        {
            return RedirectToPage("/Index");
        }
        else
        {
            if (result.RequiresTwoFactor)
            {
                return RedirectToPage("/Account/LoginTwoFactorWithAuthenticator", new { RememberMe = Credential.RememberMe });
            }
            else if (result.IsLockedOut)
            {
                ModelState.AddModelError("Login", "You are locked out.");
            }
            else
            {
                ModelState.AddModelError("Login", "Failed to login.");
            }

            return Page();
        }
    }

    public IActionResult OnPostLoginExternally(string provider)
    {
        var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, null);
        properties.RedirectUri = Url.Action("ExternalLoginCallback", "Account");

        return Challenge(properties, provider);
    }
}

public class CredentialVm
{
    [Required]
    public string Email { get; set; } = "";

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = "";

    [Display(Name = "Remember Me")]
    public bool RememberMe { get; set; }
}
