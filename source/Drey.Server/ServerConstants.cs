namespace Drey.Server
{
    public static class ServerConstants
    {
        public static class ClaimTypes
        {
            public static string Scope = "http://claims.drey.com/v1/scope";
        }

        public static class Scopes
        {
            public static string Admin = "admin";
            public static string[] All = new[] { Admin };
        }
    }
}
