namespace MemberShip.Web.Tools.Constants
{
    public static class IdentityConstants
    {
        public static class Role
        {
            public const string ADMIN = "Admin";
            public const string MANAGER = "Manager";
            public const string EDITOR = "Editor";
        }

        public static class Namer
        {
            public const string RETURN_URL = "ReturnUrl";
            public const string INDEX = "Index";
        }

        public static class Cookie
        {
            public const string NAME = "IdentityCookie";
            public const string LOGIN_PATH = "/Security/SignIn";
            public const string LOGOUT_PATH = "/Base/SignOut";
            public const string ACCESS_DENIED_PATH = "/Security/AccessDenied";
        }

        public static class Identity
        {
            public const string ALLOWED_USERNAME_CHARACTERS = "abcdefghijklmnoöpqrsştuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._";
        }

        public static class ClaimName
        {
            public const string AGE = "Age";
            public const string CITY = "City";
            public const string ISSUER = "Internal"; //Değıtıcı kim? Kim yarattı?
            public const string EXPIRE_DATE_EXCHANGE = "ExpireDateExchange";
        }

        public static class Policy
        {
            public const string CITY = "CityPolicy";
            public const string AGE = "AgePolicy";
            public const string EXCHANGE = "ExchangePolicy";
        }
    }
}
