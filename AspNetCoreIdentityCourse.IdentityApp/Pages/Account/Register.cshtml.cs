using AspNetCoreIdentityCourse.IdentityApp.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentityCourse.IdentityApp.Pages.Account;

public class RegisterModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ILogger<RegisterModel> _logger;
    private readonly IEmailService _emailService;

    public RegisterModel(UserManager<IdentityUser> userManager, ILogger<RegisterModel> logger, IEmailService emailService)
    {
        _userManager = userManager;
        _logger = logger;
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

        var user = new IdentityUser
        {
            Email = RegisterVm.Email,
            UserName = RegisterVm.Email,
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
}
