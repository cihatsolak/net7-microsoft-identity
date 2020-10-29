using MemberShip.Web.Tools.Settings;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;
using static MemberShip.Web.Tools.Constants.IdentityConstants;

namespace MemberShip.Web.TwoFactorServices
{
    public class TwoFactorService : ITwoFactorService
    {
        private readonly UrlEncoder _urlEncoder;
        private readonly IOptions<TwoFactorSettings> _twoFactorSetting;
        public TwoFactorService(UrlEncoder urlEncoder, IOptions<TwoFactorSettings> twoFactorSetting)
        {
            _urlEncoder = urlEncoder;
            _twoFactorSetting = twoFactorSetting;
        }

        public string GenerateQrCodeUri(string email, string unFormattedKey)
        {
            string encodeDomain = _urlEncoder.Encode(_twoFactorSetting.Value.Domain);
            string encodeEmail = _urlEncoder.Encode(email);

            return string.Format(QrCode.Path, encodeDomain, encodeEmail, unFormattedKey);
        }
    }
}
