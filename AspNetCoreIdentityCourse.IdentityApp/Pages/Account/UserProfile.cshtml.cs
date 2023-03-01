using AspNetCoreIdentityCourse.IdentityApp.Data.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace AspNetCoreIdentityCourse.IdentityApp.Pages.Account;

[Authorize]
public class UserProfileModel : PageModel
{
    private readonly UserManager<User> _userManager;

    [BindProperty]
    public UserProfileViewModel UserProfile { get; set; } = new();

    [BindProperty]
    public string? SuccessMessage { get; set; } = "";

    public UserProfileModel(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var (user, aliasClaim) = await GetUserAndAliasClaimAsync();

        UserProfile.Email = User.Identity?.Name ?? "";
        UserProfile.Department = user.Department;
        UserProfile.Position = user.Position;
        UserProfile.Alias = aliasClaim?.Value ?? "";

        SuccessMessage = "";

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var (user, aliasClaim) = await GetUserAndAliasClaimAsync();

        user.Department = UserProfile.Department;
        user.Position = UserProfile.Position;

        try
        {
            await _userManager.UpdateAsync(user);
            await _userManager.ReplaceClaimAsync(user, aliasClaim, new Claim(aliasClaim.Type, UserProfile.Alias));
            SuccessMessage = "The user profile is saved successfully";
        }
        catch
        {
            ModelState.AddModelError("UserProfile", "Error occurred when saving user profile.");
        }

        return Page();
    }

    private async Task<(User, Claim)> GetUserAndAliasClaimAsync()
    {
        var user = await _userManager.FindByNameAsync(User.Identity?.Name ?? "");
        var claims = await _userManager.GetClaimsAsync(user!);
        var aliasClaim = claims.First(x => x.Type == "Alias");
        return (user!, aliasClaim);
    }
}

public class UserProfileViewModel
{
    [Required]
    public string Email { get; set; } = "";

    [Required]
    public string Department { get; set; } = "";

    [Required]
    public string Position { get; set; } = "";

    [Required]
    public string Alias { get; set; } = "";
}
