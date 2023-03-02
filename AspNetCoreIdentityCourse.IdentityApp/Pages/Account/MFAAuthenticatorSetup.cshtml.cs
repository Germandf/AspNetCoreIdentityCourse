using AspNetCoreIdentityCourse.IdentityApp.Data.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentityCourse.IdentityApp.Pages.Account;

[Authorize]
public class MFAAuthenticatorSetupModel : PageModel
{
    private readonly UserManager<User> _userManager;

    [BindProperty]
    public SetupMFAVm SetupMFA { get; set; } = new();

    [BindProperty]
    public bool Succeeded { get; set; } = false;

    public MFAAuthenticatorSetupModel(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);

        await _userManager.ResetAuthenticatorKeyAsync(user!);

        var key = await _userManager.GetAuthenticatorKeyAsync(user!);

        SetupMFA.Key = key!;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var user = await _userManager.GetUserAsync(User);

        if (await _userManager.VerifyTwoFactorTokenAsync(
            user!, _userManager.Options.Tokens.AuthenticatorTokenProvider, SetupMFA.SecurityCode))
        {
            await _userManager.SetTwoFactorEnabledAsync(user!, true);
            Succeeded = true;
        }
        else
        {
            ModelState.AddModelError("AuthenticatorSetup", "Something went wrong with authenticator setup.");
        }

        return Page();
    }
}

public class SetupMFAVm
{
    public string Key { get; set; } = "";

    [Required]
    [DisplayName("Security Code")]
    public string SecurityCode { get; set; } = "";
}
