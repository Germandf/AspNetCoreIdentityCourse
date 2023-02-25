using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace AspNetCoreIdentityCourse.Pages.Account;

public class LoginModel : PageModel
{
    [BindProperty]
    public Credential Credential { get; set; } = new();

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid || Credential.UserName != "admin" || Credential.Password != "password")
        {
            return Page();
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, "admin"),
            new Claim(ClaimTypes.Email, "admin@mywebsite.com"),
            new Claim("Department", "HR"),
            new Claim("Admin", "true"),
            new Claim("Manager", "true"),
        };
        var identity = new ClaimsIdentity(claims, "MyCookieAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync("MyCookieAuth", claimsPrincipal);

        return RedirectToPage("/Index");
    }
}

public class Credential
{
    [Required]
    [Display(Name = "User Name")]
    public string UserName { get; set; } = "";

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = "";
}
