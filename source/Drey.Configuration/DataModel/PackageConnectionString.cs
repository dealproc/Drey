namespace Drey.Configuration.DataModel
{
    class PackageConnectionString
    {
        public RegisteredPackage Package { get; set; }
        public string Name { get; set; }
        public string ConnectionString { get; set; }
        public string ProviderName { get; set; }
    }
}