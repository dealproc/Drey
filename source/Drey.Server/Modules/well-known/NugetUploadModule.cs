using Nancy;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Drey.Server.Modules.well_known
{
    public class NugetUploadModule : NancyModule
    {
        const string NUGET_DIR = @"c:\packages";

        public NugetUploadModule() : base("/api/v2/package")
        {
            Put["/", runAsync: true] = ParseAndStorePackage;
        }

        private async Task<dynamic> ParseAndStorePackage(dynamic _, CancellationToken ct)
        {
            if (!Request.Files.Any())
            {
                return ((Response)"Missing file").StatusCode = HttpStatusCode.BadRequest;
            }

            NuGet.ZipPackage pkg = new NuGet.ZipPackage(Request.Files.First().Value);

            if (!Directory.Exists(NUGET_DIR)) { Directory.CreateDirectory(NUGET_DIR); }

            var fileName = Path.Combine(NUGET_DIR, string.Format("{0}-{1}.nupkg", pkg.Id, pkg.Version));

            if (File.Exists(fileName)) { File.Delete(fileName); }

            using (var pkgStore = File.OpenWrite(fileName))
            {
                await pkg.GetStream().CopyToAsync(pkgStore);
            }

            return HttpStatusCode.Created;
        }
    }
}