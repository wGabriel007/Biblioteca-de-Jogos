using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Configuration;

namespace Biblioteca_de_Jogos.Services
{
    public interface IEmailService
    {
        Task EnviarAsync(string destinatario, string assunto, string corpo);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task EnviarAsync(string destinatario, string assunto, string corpo)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_config["Email:Remetente"]));
            email.To.Add(MailboxAddress.Parse(destinatario));
            email.Subject = assunto;
            email.Body = new TextPart("html") { Text = corpo };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_config["Email:SmtpHost"],
                                    int.Parse(_config["Email:SmtpPort"]!),
                                    MailKit.Security.SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_config["Email:Remetente"],
                                         _config["Email:SenhaApp"]);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}
