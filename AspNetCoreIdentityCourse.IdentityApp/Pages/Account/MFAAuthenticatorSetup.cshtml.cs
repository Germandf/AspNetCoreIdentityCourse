using AspNetCoreIdentityCourse.IdentityApp.Data.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QRCoder;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentityCourse.IdentityApp.Pages.Account;

[Authorize]
public class MFAAuthenticatorSetupModel : PageModel
{
    private readonly UserManager<User> _userManager;

    [BindProperty]
    public SetupMFAVm SetupMFA { get; set; } = new();

    [BindProperty]
    public bool Succeeded { get; set; } = false;

    public MFAAuthenticatorSetupModel(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);

        await _userManager.ResetAuthenticatorKeyAsync(user!);

        var key = await _userManager.GetAuthenticatorKeyAsync(user!);

        SetupMFA.Key = key!;
        SetupMFA.QRCodeBytes = GenerateQRCodeBytes("AspNetCoreIdentityCourse", key!, user!.Email!);
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var user = await _userManager.GetUserAsync(User);

        if (await _userManager.VerifyTwoFactorTokenAsync(
            user!, _userManager.Options.Tokens.AuthenticatorTokenProvider, SetupMFA.SecurityCode))
        {
            await _userManager.SetTwoFactorEnabledAsync(user!, true);
            Succeeded = true;
        }
        else
        {
            ModelState.AddModelError("AuthenticatorSetup", "Something went wrong with authenticator setup.");
        }

        return Page();
    }

    private byte[] GenerateQRCodeBytes(string provider, string key, string userEmail)
    {
        var qrCodeGenerator = new QRCodeGenerator();

        var qrCodeData = qrCodeGenerator.CreateQrCode(
            $"otpauth://totp/{provider}:{userEmail}?secret={key}&issuer={provider}",
            QRCodeGenerator.ECCLevel.Q);

        var qrCode = new PngByteQRCode(qrCodeData);
        var qrCodeImage = qrCode.GetGraphic(20);
        return qrCodeImage;
    }
}

public class SetupMFAVm
{
    public string Key { get; set; } = "";

    [Required]
    [DisplayName("Security Code")]
    public string SecurityCode { get; set; } = "";

    public byte[] QRCodeBytes { get; set; } = Array.Empty<byte>();
}
