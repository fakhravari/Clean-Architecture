using Application.Contracts.Infrastructure;
using Domain.Model.Email;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Infrastructure.Email;

public class EmailRepository : IEmailRepository
{
    private readonly EmailSettingModel smtp;

    public EmailRepository(IOptions<EmailSettingModel> settings)
    {
        smtp = settings.Value;
    }

    public async Task<bool> SendEmailAsync(string toEmail, string subject, string body)
    {
        try
        {
            var message = new MailMessage
            {
                From = new MailAddress(smtp.FromEmail, smtp.FromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = false,
                BodyEncoding = Encoding.UTF8,
                Priority = MailPriority.High
            };

            message.To.Add(toEmail);

            var smtpMail = new SmtpClient
            {
                Host = smtp.Host,
                Port = smtp.Port,
                Credentials = new NetworkCredential(smtp.UserName, smtp.Password),
                EnableSsl = smtp.EnableSsl,
            };

            smtpMail.ServicePoint.SetTcpKeepAlive(true, 2000, 2000);

            await smtpMail.SendMailAsync(message);

            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

}