using System.Threading.Tasks;

namespace MemberShip.Web.Services.SendGridServices
{
    public interface ISendGridService
    {
        public Task SendEmailVerificationAsync(string email, string fullName, string url);
        public Task SendPasswordResetEmailAsync(string email, string fullName, string url);
        public Task<int> SendVerificationCodeAsync(string email, string fullName);
    }
}
