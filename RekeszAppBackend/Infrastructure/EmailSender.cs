using System.Net;
using System.Net.Mail;

namespace RekeszAppBackend.Infrastructure;

public interface IEmailSender
{
    Task SendAsync(string recipient, string subject, string htmlBody, CancellationToken cancellationToken = default);
}

public sealed class SmtpEmailSender(IConfiguration configuration) : IEmailSender
{
    public async Task SendAsync(string recipient, string subject, string htmlBody, CancellationToken cancellationToken = default)
    {
        var host = configuration["Smtp:Host"];
        var portValue = configuration["Smtp:Port"];
        var user = configuration["Smtp:Username"];
        var password = configuration["Smtp:Password"];
        var from = configuration["Smtp:FromAddress"];
        var fromName = configuration["Smtp:FromName"] ?? "ZoldPiac";

        if (string.IsNullOrWhiteSpace(host) || !int.TryParse(portValue, out var port) ||
            string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(password) ||
            string.IsNullOrWhiteSpace(from))
        {
            throw new InvalidOperationException("Az email küldéshez az Smtp:Host, Port, Username, Password és FromAddress beállítása szükséges.");
        }

        using var message = new MailMessage
        {
            From = new MailAddress(from, fromName),
            Subject = subject,
            Body = htmlBody,
            IsBodyHtml = true
        };
        message.To.Add(recipient);

        using var smtp = new SmtpClient(host, port)
        {
            EnableSsl = bool.TryParse(configuration["Smtp:EnableSsl"], out var ssl) && ssl,
            Credentials = new NetworkCredential(user, password),
            DeliveryMethod = SmtpDeliveryMethod.Network
        };

        cancellationToken.ThrowIfCancellationRequested();
        await smtp.SendMailAsync(message, cancellationToken);
    }
}
