namespace MemberShip.Web.Tools
{
    public static class ErrorMessage
    {
        public const string USER_NOT_FOUND = "E-posta adresine bağlı bir kullanıcı bulunamadı.";
        public const string USER_LOCKOUT = "Hesabınız bir süreliğine kilitlenmiştir. Lütfen daha sonra tekrar deneyiniz.";
        public const string FAILED_LOGIN = "Giriş başarısız.";
        public const string INCORRECT_LOGIN = "Hesabınız çok sayıda başarısız giriş denemesiyle nedeniyle kilitlenmiştir.";
        public const string PASSWORD_WRONG = "Şifrenizi kontrol ediniz.";
        public const string EMAIL_NOT_VERIFIED = "Email adresiniz doğrulanmamıştır. Lütfen email adresinizi doğrulayınız.";
    }
}
