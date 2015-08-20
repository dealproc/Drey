using System;
using System.Collections.Generic;
namespace Drey.Configuration.Services
{
    public interface IPackageService
    {
        IEnumerable<DataModel.Release> Diff(string packageId, IEnumerable<DataModel.Release> discoveredReleases);
        IEnumerable<DataModel.Package> GetPackages();
        IEnumerable<DataModel.Release> GetReleases(string packageId);
        void RecordReleases(System.Collections.Generic.IEnumerable<DataModel.Release> newReleases);
    }
}