using AspNetCoreIdentityCourse.IdentityApp.Data.Account;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AspNetCoreIdentityCourse.IdentityApp.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly SignInManager<User> _signInManager;

    public AccountController(SignInManager<User> signInManager)
    {
        _signInManager = signInManager;
    }

    public async Task<IActionResult> ExternalLoginCallback()
    {
        var loginInfo = await _signInManager.GetExternalLoginInfoAsync();
        var emailClaim = loginInfo!.Principal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);
        var userClaim = loginInfo!.Principal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name);

        if (emailClaim is not null && userClaim is not null)
        {
            var user = new User { Email = emailClaim.Value, UserName = userClaim.Value, Department = "None", Position = "None" };
            await _signInManager.SignInAsync(user, false);
        }

        return RedirectToPage("/Index");
    }
}
