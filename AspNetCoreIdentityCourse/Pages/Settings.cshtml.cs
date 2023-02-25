using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AspNetCoreIdentityCourse.Pages;

[Authorize(Policy = "AdminOnly")]
public class SettingsModel : PageModel
{
    public void OnGet()
    {
    }
}
