namespace Drey.Server
{
    public static class ServerConstants
    {
        public static class ClaimTypes
        {
            /// <summary>
            /// A scope attribute, to express scopes used by the drey server components.
            /// </summary>
            public static string Scope = "http://claims.drey.com/v1/scope";

            /// <summary>
            /// When set on a claimsprincipal, expresses the api key the principal may use to publish nuget packages to the reflector (for client auto-updates)
            /// </summary>
            public static string NugetApiKey = "http://claims.drey.com/v1/nugetapikey";
        }

        public static class Scopes
        {
            public static string Admin = "admin";
            public static string[] All = new[] { Admin };
        }
    }
}
