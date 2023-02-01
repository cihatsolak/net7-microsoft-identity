using MemberShip.Web.Tools.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Text.Encodings.Web;
using static MemberShip.Web.Tools.Constants.IdentityConstants;

namespace MemberShip.Web.Services.TwoFactorServices
{
    public class TwoFactorService : ITwoFactorService
    {
        private readonly UrlEncoder _urlEncoder;
        private readonly IOptions<TwoFactorSettings> _twoFactorSetting;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public TwoFactorService(UrlEncoder urlEncoder, IOptions<TwoFactorSettings> twoFactorSetting, IHttpContextAccessor httpContextAccessor)
        {
            _urlEncoder = urlEncoder;
            _twoFactorSetting = twoFactorSetting;
            _httpContextAccessor = httpContextAccessor;
        }

        public string GenerateQrCodeUri(string email, string unFormattedKey)
        {
            string encodeDomain = _urlEncoder.Encode(_twoFactorSetting.Value.Domain);
            string encodeEmail = _urlEncoder.Encode(email);

            return string.Format(QrCode.Path, encodeDomain, encodeEmail, unFormattedKey);
        }

        public int GetCodeVerification()
        {
            int code = new Random().Next(1000, 9999);
            _httpContextAccessor.HttpContext.Session.SetInt32(Namer.VERIFICATION_CODE, code);
            return code;
        }

        public int TimeLeft()
        {
            string sessionCurrentTime = _httpContextAccessor.HttpContext.Session.GetString(Namer.CURRENT_TIME);

            if (string.IsNullOrEmpty(sessionCurrentTime))
            {
                _httpContextAccessor.HttpContext.Session.SetString(Namer.CURRENT_TIME, DateTime.Now.AddSeconds(_twoFactorSetting.Value.CodeTimeExpire).ToString());
            }

            sessionCurrentTime = _httpContextAccessor.HttpContext.Session.GetString(Namer.CURRENT_TIME);

            DateTime currentTime = DateTime.Parse(sessionCurrentTime);

            int totalSeconds = (int)(currentTime - DateTime.Now).TotalSeconds;

            if (0 >= totalSeconds)
            {
                _httpContextAccessor.HttpContext.Session.Remove(Namer.CURRENT_TIME);
                return 0;
            }

            return totalSeconds;
        }
    }
}
