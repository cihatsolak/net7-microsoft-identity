using MemberShip.Web.Services.TwoFactorServices;
using MemberShip.Web.Tools.Settings;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net;
using System.Threading.Tasks;

namespace MemberShip.Web.Services.SendGridServices
{
    public class SendGridService : ISendGridService
    {
        private readonly ITwoFactorService _twoFactorService;
        private readonly IOptions<SendGridSettings> _sendGridSettings;

        public SendGridService(ITwoFactorService twoFactorService, IOptions<SendGridSettings> sendGridSettings)
        {
            _twoFactorService = twoFactorService;
            _sendGridSettings = sendGridSettings;
        }

        public async Task SendEmailVerificationAsync(string email, string fullName, string url)
        {
            string subject = "www.identity.com::Email Doğrulama";
            string htmlContent = "<h2>ŞEmail adresini doğrulamak için linke tıklayınız.<h2>";
            htmlContent += $"<a href='{url}'>Email doğrulama linki</a>";

            await SendAsync(email, subject, fullName, null, htmlContent);
        }

        public async Task SendPasswordResetEmailAsync(string email, string fullName, string url)
        {
            string subject = "www.identity.com::Şifre Sıfırlama";
            string htmlContent = "<h2>Şifrenizi yenilemek için linke tıklayınız.<h2>";
            htmlContent += $"<a href='{url}'>Şifre yenileme linki</a>";

            await SendAsync(email, subject, fullName, null, htmlContent);
        }

        public async Task<int> SendVerificationCodeAsync(string email, string fullName)
        {
            int code = _twoFactorService.GetCodeVerification();

            string subject = "İki Adımlı Kimlik Doğrulama Kodunuz";

            string htmlContent = "<h2>Siteye giriş yapabilmek için doğrulama kodunuz aşağıdadır.</h2>";
            htmlContent += $"<h3>Kodunuz: {code}</h3>";

            var result = await SendAsync(email, subject, fullName, null, htmlContent);

            return result ? code : -1;
        }

        private async Task<bool> SendAsync(string email, string subject, string fullName, string plainTextContent, string htmlContent)
        {
            var apiKey = _sendGridSettings.Value.ApiKey;
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("ogreniyorum@identity.com", "Identity Ögreniyorum");
            var to = new EmailAddress(email, $"{fullName}");
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);

            return response.StatusCode == HttpStatusCode.OK;
        }
    }
}
