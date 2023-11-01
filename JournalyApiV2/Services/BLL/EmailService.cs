using JournalyApiV2.Models;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace JournalyApiV2.Services.BLL;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendVerificationEmailAsync(string toEmail, string firstName, string lastName, EmailVerification codes)
    {
        var msg = new SendGridMessage();
        msg.SetTemplateId(_config.GetValue<string>("SendGrid:Templates:EmailVerification"));
        var data = new
        {
            name = firstName,
            code = codes.ShortCode,
            verificationLink = "https://link.journaly.io/email?" + codes.LongCode
        };
        msg.SetTemplateData(data);

        await SendEmailAsync(toEmail, $"{firstName} {lastName}", msg);
    }
    
    public async Task SendPasswordResetEmailAsync(string toEmail, string firstName, string lastName, string code)
    {
        var msg = new SendGridMessage();
        msg.SetTemplateId(_config.GetValue<string>("SendGrid:Templates:PasswordReset"));
        var data = new
        {
            name = firstName,
            verificationLink = "https://link.journaly.io/password?" + code
        };
        msg.SetTemplateData(data);

        await SendEmailAsync(toEmail, $"{firstName} {lastName}", msg);
    }
    
    private async Task SendEmailAsync(string toEmail, string toName, SendGridMessage message)
    {
        var client = new SendGridClient(_config.GetValue<string>("SendGrid:ApiKey"));

        var from = new EmailAddress(_config.GetValue<string>("SendGrid:FromEmail"), _config.GetValue<string>("SendGrid:Name"));
        var to = new EmailAddress(toEmail, toName);
        
        message.SetFrom(from);
        message.AddTo(to);
        
        var response = await client.SendEmailAsync(message);

        if (response.StatusCode != System.Net.HttpStatusCode.Accepted)
        {
            throw new Exception($"SendGrid error when sending email: {response.StatusCode}");
        }
    }
}