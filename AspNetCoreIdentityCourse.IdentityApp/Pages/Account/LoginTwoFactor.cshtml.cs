using AspNetCoreIdentityCourse.IdentityApp.Data.Account;
using AspNetCoreIdentityCourse.IdentityApp.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentityCourse.IdentityApp.Pages.Account;

public class LoginTwoFactorModel : PageModel
{
    private readonly UserManager<User> _userManager;
    private readonly IEmailService _emailService;
    private readonly SignInManager<User> _signInManager;

    [BindProperty]
    public Email2FAVm Email2FA { get; set; } = new();

    public LoginTwoFactorModel(
        UserManager<User> userManager, 
        IEmailService emailService, 
        SignInManager<User> signInManager)
    {
        _userManager = userManager;
        _emailService = emailService;
        _signInManager = signInManager;
    }

    public async Task<IActionResult> OnGetAsync(string email, bool rememberMe)
    {
        Email2FA.RememberMe = rememberMe;

        var user = await _userManager.FindByEmailAsync(email);

        var token = await _userManager.GenerateTwoFactorTokenAsync(user!, "Email");

        await _emailService.SendAsync("noreply@identityapp.com", email, 
            "My Web App's 2FA", 
            $"Pease use this code as the 2FA: {token}");

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var result = await _signInManager.TwoFactorSignInAsync(
            "Email", Email2FA.SecurityCode, Email2FA.RememberMe, false);

        if (result.Succeeded)
        {
            return RedirectToPage("/Index");
        }
        else
        {
            if (result.IsLockedOut)
            {
                ModelState.AddModelError("Login2FA", "You are locked out.");
            }
            else
            {
                ModelState.AddModelError("Login2FA", "Failed to login.");
            }

            return Page();
        }
    }
}

public class Email2FAVm
{
    [Required]
    [DisplayName("Security Code")]
    public string SecurityCode { get; set; } = "";

    public bool RememberMe { get; set; }
}
