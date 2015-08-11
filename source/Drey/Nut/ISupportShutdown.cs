using System.Threading.Tasks;

namespace Drey.Nut
{
    public interface ISupportShutdown
    {
        bool AttemptShutdown();
    }
}