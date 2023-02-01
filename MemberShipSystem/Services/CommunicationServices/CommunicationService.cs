using MemberShip.Web.Services.TwoFactorServices;
using MemberShip.Web.Tools.Settings;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net;
using System.Threading.Tasks;

namespace MemberShip.Web.Services.SendGridServices
{
    public class CommunicationService : ICommunicationService
    {
        private readonly ITwoFactorService _twoFactorService;
        private readonly IOptions<SendGridSettings> _sendGridSettings;

        public CommunicationService(ITwoFactorService twoFactorService, IOptions<SendGridSettings> sendGridSettings)
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

        public async Task<int> SendEmailVerificationCodeAsync(string email, string fullName)
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
            var from = new EmailAddress(_sendGridSettings.Value.From, _sendGridSettings.Value.Name);
            var to = new EmailAddress(email, $"{fullName}");
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);

            if (response.StatusCode == HttpStatusCode.Accepted || response.StatusCode == HttpStatusCode.OK)
                return true;

            return false;
        }

        public async Task<int> SendSmsVerificationCodeAsync(string phoneNumber, string fullName)
        {
            //Sms api ücretli olduğu için burayı kodlamadık.

            int code = _twoFactorService.GetCodeVerification();

            await SendAsync("", "", "", "", "");

            return - 1;
        }
    }
}
