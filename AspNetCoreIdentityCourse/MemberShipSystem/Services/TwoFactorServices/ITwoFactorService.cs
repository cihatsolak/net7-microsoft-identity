namespace MemberShip.Web.Services.TwoFactorServices
{
    public interface ITwoFactorService
    {
        int TimeLeft();
        int GetCodeVerification();
        string GenerateQrCodeUri(string email, string unFormattedKey);
    }
}
