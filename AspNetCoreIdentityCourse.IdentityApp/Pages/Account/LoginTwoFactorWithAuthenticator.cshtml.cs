using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Microsoft.AspNetCore.Identity;
using AspNetCoreIdentityCourse.IdentityApp.Data.Account;

namespace AspNetCoreIdentityCourse.IdentityApp.Pages.Account;

public class LoginTwoFactorWithAuthenticatorModel : PageModel
{
    private readonly SignInManager<User> _signInManager;

    public LoginTwoFactorWithAuthenticatorModel(SignInManager<User> signInManager)
    {
        _signInManager = signInManager;
    }

    [BindProperty]
    public Authenticator2FAVm Authenticator2FA { get; set; } = new();

    public void OnGet(bool rememberMe)
    {
        Authenticator2FA.SecurityCode = "";
        Authenticator2FA.RememberMe = rememberMe;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(
            Authenticator2FA.SecurityCode, Authenticator2FA.RememberMe, false);

        if (result.Succeeded)
        {
            return RedirectToPage("/Index");
        }
        else
        {
            if (result.IsLockedOut)
            {
                ModelState.AddModelError("Authenticator2FA", "You are locked out.");
            }
            else
            {
                ModelState.AddModelError("Authenticator2FA", "Failed to login.");
            }

            return Page();
        }
    }
}

public class Authenticator2FAVm
{
    [Required]
    [DisplayName("Security Code")]
    public string SecurityCode { get; set; } = "";

    public bool RememberMe { get; set; }
}
