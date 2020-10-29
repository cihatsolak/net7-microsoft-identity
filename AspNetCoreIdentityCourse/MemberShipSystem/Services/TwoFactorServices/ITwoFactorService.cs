namespace MemberShip.Web.Services.TwoFactorServices
{
    public interface ITwoFactorService
    {
        int GetCodeVerification();
        string GenerateQrCodeUri(string email, string unFormattedKey);
    }
}
