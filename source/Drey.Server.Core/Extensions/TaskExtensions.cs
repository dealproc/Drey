using System.Threading.Tasks;

namespace Drey.Server.Extensions
{
    public static class TaskExtensions
    {
        /// <summary>
        /// Casts a Task{T} to a Task{dynamic} for returning properly via a Nancy module.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task">The task.</param>
        /// <returns>a Task{dynamic}</returns>
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
