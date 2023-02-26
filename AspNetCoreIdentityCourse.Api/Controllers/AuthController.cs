using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AspNetCoreIdentityCourse.Api.Controllers;

[Route("[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public AuthController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpPost]
    public IActionResult Authenticate([FromBody] Credential credential)
    {
        if (!ModelState.IsValid || credential.UserName != "admin" || credential.Password != "password")
        {
            ModelState.AddModelError("Unauthorized", "You are not authorized to access the endpoint.");
            return Unauthorized(ModelState);
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, "admin"),
            new Claim(ClaimTypes.Email, "admin@mywebsite.com"),
            new Claim("Department", "HR"),
            new Claim("Admin", "true"),
            new Claim("Manager", "true"),
            new Claim("EmploymentDate", "2022-11-01")
        };

        var expiresAt = DateTime.UtcNow.AddMinutes(10);

        return Ok(new
        {
            access_token = CreateToken(claims, expiresAt),
            expires_at = expiresAt,
        });
    }

    private string CreateToken(IEnumerable<Claim> claims, DateTime expiresAt)
    {
        var secretKey = Encoding.ASCII.GetBytes(_configuration.GetValue<string>("SecretKey")!);

        var jwt = new JwtSecurityToken(
            claims: claims, 
            notBefore: DateTime.UtcNow, 
            expires: expiresAt, 
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(secretKey),
                SecurityAlgorithms.HmacSha256Signature));

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
}

public class Credential
{
    public string UserName { get; set; } = "";
    public string Password { get; set; } = "";
}