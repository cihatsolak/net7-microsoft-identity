using System.Text.Encodings.Web;
using static MemberShip.Web.Tools.Constants.IdentityConstants;

namespace MemberShip.Web.TwoFactorServices
{
    public class TwoFactorService : ITwoFactorService
    {
        private readonly UrlEncoder _urlEncoder;
        public TwoFactorService(UrlEncoder urlEncoder)
        {
            _urlEncoder = urlEncoder;
        }

        public string GenerateQrCodeUri(string email, string unFormattedKey)
        {
            string encodeDomain = _urlEncoder.Encode("www.identity.com");
            string encodeEmail = _urlEncoder.Encode(email);

            return string.Format(QrCode.Path, encodeDomain, encodeEmail, unFormattedKey);
        }
    }
}
