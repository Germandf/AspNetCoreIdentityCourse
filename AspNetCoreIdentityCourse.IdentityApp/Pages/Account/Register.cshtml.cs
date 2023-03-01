using AspNetCoreIdentityCourse.IdentityApp.Data.Account;
using AspNetCoreIdentityCourse.IdentityApp.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentityCourse.IdentityApp.Pages.Account;

public class RegisterModel : PageModel
{
    private readonly UserManager<User> _userManager;
    private readonly IEmailService _emailService;

    public RegisterModel(UserManager<User> userManager, IEmailService emailService)
    {
        _userManager = userManager;
        _emailService = emailService;
    }

    [BindProperty]
    public RegisterVm RegisterVm { get; set; } = new();

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var user = new User
        {
            Email = RegisterVm.Email,
            UserName = RegisterVm.Email,
            Department = RegisterVm.Department,
            Position = RegisterVm.Position,
        };

        var result = await _userManager.CreateAsync(user, RegisterVm.Password);

        if (result.Succeeded)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var link = Url.PageLink(pageName: "/Account/ConfirmEmail", values: new { userId = user.Id, token });
            
            await _emailService.SendAsync("noreply@identityapp.com", user.Email, 
                "Please confirm your email", 
                $"Please click on this link to confirm your email address: {link}");

            return RedirectToPage("/Account/Login");
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

    [Required]
    public string Department { get; set; } = "";

    [Required]
    public string Position { get; set; } = "";
}
