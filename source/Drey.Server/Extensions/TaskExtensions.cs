using System.Threading.Tasks;

namespace Drey.Server.Extensions
{
    static class TaskExtensions
    {
        public static Task<dynamic> AsDynamicTask<T>(this Task<T> task)
        {
            return task.ContinueWith(continuation =>
            {
                dynamic result = continuation.Result;
                return result;
            });
        }
    }
}
