using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AspNetCoreIdentityCourse.Pages;

[Authorize(Policy = "MustBelongToHRDepartment")]
public class HumanResourcesModel : PageModel
{
    public void OnGet()
    {
    }
}
