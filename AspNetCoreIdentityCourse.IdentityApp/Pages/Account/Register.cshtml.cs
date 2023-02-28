using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentityCourse.IdentityApp.Pages.Account;

public class RegisterModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;

    public RegisterModel(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    [BindProperty]
    public RegisterVm RegisterVm { get; set; } = new();

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var user = new IdentityUser
        {
            Email = RegisterVm.Email,
            UserName = RegisterVm.Email,
        };

        var result = await _userManager.CreateAsync(user, RegisterVm.Password);

        if (result.Succeeded)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            return Redirect(Url.PageLink(pageName: "/Account/ConfirmEmail", values: new { userId = user.Id, token })!);
        }
        else
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("Register", error.Description);
            }

            return Page();
        }
    }
}

public class RegisterVm
{
    [Required]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email { get; set; } = "";

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = "";
}
