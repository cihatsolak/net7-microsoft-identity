namespace AspNetCoreIdentityApp.Web.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> options)
        {
            _emailSettings = options.Value;
        }

        public async Task SendResetPasswordEmailAsync(string resetPasswordEmailLink, string ToEmail)
        {
            var smptClient = new SmtpClient
            {
                Host = _emailSettings.Host,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Port = 587,
                Credentials = new NetworkCredential(_emailSettings.Email, _emailSettings.Password),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_emailSettings.Email)
            };
            mailMessage.To.Add(ToEmail);

            mailMessage.Subject = "Localhost | Şifre sıfırlama linki";
            mailMessage.Body = @$"
                  <h4>Şifrenizi yenilemek için aşağıdaki linkte tıklayınız.</h4>
                  <p><a href='{resetPasswordEmailLink}'>şifre yenileme link</a></p>";

            mailMessage.IsBodyHtml = true;

            await smptClient.SendMailAsync(mailMessage);
        }
    }
}
