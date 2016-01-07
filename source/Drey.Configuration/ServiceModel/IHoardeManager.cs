using System;
namespace Drey.Configuration.ServiceModel
{
    public interface IHoardeManager : IDisposable
    {
        void Handle(Drey.Nut.ShellRequestArgs e);
        bool IsOnline(DataModel.Release package);
    }
}
