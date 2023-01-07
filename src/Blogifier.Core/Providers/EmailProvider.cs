using Blogifier.Shared;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blogifier.Core.Providers
{
    public interface IEmailProvider
    {
        Task<bool> SendEmail(MailSetting settings, List<Subscriber> subscribers, string subject, string content);
    }

    public class MailKitProvider : IEmailProvider
    {
        public MailKitProvider() { }

        public async Task<bool> SendEmail(MailSetting settings, List<Subscriber> subscribers, string subject, string content)
        {
            var client = await GetClientAsync(settings);
            if (client == null)
                return false;

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = content;

            foreach (var subscriber in subscribers)
            {
                try
                {
                    var message = new MimeMessage();
                    message.Subject = subject;
                    message.Body = bodyBuilder.ToMessageBody();
                    message.From.Add(new MailboxAddress(settings.FromName, settings.FromEmail));
                    message.To.Add(new MailboxAddress(settings.ToName, subscriber.Email));
                    await client.SendAsync(message);
                }
                catch (Exception ex)
                {
                    Serilog.Log.Warning($"Error sending email to {subscriber.Email}: {ex.Message}");
                }
            }

            await client.DisconnectAsync(true);
            return true;
        }

        private async Task<SmtpClient> GetClientAsync(MailSetting settings)
        {
            try
            {
                var client = new SmtpClient();
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                await client.ConnectAsync(settings.Host, settings.Port, SecureSocketOptions.Auto);
                await client.AuthenticateAsync(settings.UserEmail, settings.UserPassword);

                return client;
            }
            catch (Exception ex)
            {
                Serilog.Log.Error($"Error connecting to SMTP client: {ex.Message}");
                return null;
            }
        }
    }
}
