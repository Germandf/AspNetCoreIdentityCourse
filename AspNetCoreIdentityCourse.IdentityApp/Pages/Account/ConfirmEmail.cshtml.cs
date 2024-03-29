using AspNetCoreIdentityCourse.IdentityApp.Data.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AspNetCoreIdentityCourse.IdentityApp.Pages.Account;

public class ConfirmEmailModel : PageModel
{
    private readonly UserManager<User> _userManager;

    [BindProperty]
    public string Message { get; set; } = "";

    public ConfirmEmailModel(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IActionResult> OnGetAsync(string userId, string token)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user is not null)
        {
            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (result.Succeeded)
            {
                Message = "Email address is successfully confirm, now you can try to login.";
                return Page();
            }
        }

        Message = "Failed to validate email.";
        return Page();
    }
}
