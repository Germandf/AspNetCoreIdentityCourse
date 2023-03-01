using Microsoft.AspNetCore.Identity;

namespace AspNetCoreIdentityCourse.IdentityApp.Data.Account;

public class User : IdentityUser
{
    public required string Department { get; set; }
    public required string Position { get; set; }
}
