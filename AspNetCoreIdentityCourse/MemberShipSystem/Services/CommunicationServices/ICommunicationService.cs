using System.Threading.Tasks;

namespace MemberShip.Web.Services.SendGridServices
{
    public interface ICommunicationService
    {
        public Task SendEmailVerificationAsync(string email, string fullName, string url);
        public Task SendPasswordResetEmailAsync(string email, string fullName, string url);
        public Task<int> SendEmailVerificationCodeAsync(string email, string fullName);
        public Task<int> SendSmsVerificationCodeAsync(string phoneNumber, string fullName);
    }
}
