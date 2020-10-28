namespace MemberShip.Web.TwoFactorServices
{
    public interface ITwoFactorService
    {
        string GenerateQrCodeUri(string email, string unFormattedKey);
    }
}
