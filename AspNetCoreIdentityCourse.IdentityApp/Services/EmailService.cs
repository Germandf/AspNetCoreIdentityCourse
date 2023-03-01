using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Options;
using AspNetCoreIdentityCourse.IdentityApp.Settings;

namespace AspNetCoreIdentityCourse.IdentityApp.Services;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;
    private readonly SmtpSettings _smtpSettings;

    public EmailService(ILogger<EmailService> logger, IOptions<SmtpSettings> smtpSettings)
    {
        _logger = logger;
        _smtpSettings = smtpSettings.Value;
    }

    public async Task SendAsync(string from, string to, string subject, string body)
    {
        var message = new MailMessage(from, to, subject, body);

        try
        {
            using var smtpClient = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port);
            smtpClient.Credentials = new NetworkCredential(_smtpSettings.User, _smtpSettings.Password);
            await smtpClient.SendMailAsync(message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending confirmation email");

            _logger.LogInformation(@$"
                Email tried to send: {Environment.NewLine}
                From: {from} {Environment.NewLine}
                To: {to} {Environment.NewLine}
                Subject: {subject} {Environment.NewLine}
                Body: {body} {Environment.NewLine}");
        }
    }
}
