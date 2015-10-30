using Drey.Logging;

using Microsoft.AspNet.SignalR.Client;

using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Drey.Configuration.ServiceModel
{
    abstract class RemoteInvocationService<TRequest, TRequestMessage, TResponse, TResponseMessage> : IRemoteInvocationService<TRequest, TResponse>
        where TRequest : DomainModel.Request<TRequestMessage>
        where TResponse : DomainModel.Response<TResponseMessage>
    {
        protected static ILog Log { get; private set; }
        static RemoteInvocationService()
        {
            Log = LogProvider.GetCurrentClassLogger();
        }

        readonly string _eventName;
        readonly string _remoteMethodName;
        IHubProxy _runtimeHubProxy;

        public RemoteInvocationService(string eventName, string remoteMethodName)
        {
            _eventName = eventName;
            _remoteMethodName = remoteMethodName;
        }

        public void SubscribeToEvents(IHubProxy runtimeHubProxy)
        {
            _runtimeHubProxy = runtimeHubProxy;
            Log.DebugFormat("Subscribing to {event} which will invoke {remoteMethod} upon completion.", _eventName, _remoteMethodName);
            _runtimeHubProxy.On<TRequest>(_eventName, (request) =>
            {
                Log.DebugFormat("Received {event} from server.", _eventName);
                Task.Factory.StartNew(async () =>
                {
                    TResponse response = default(TResponse);
                    Stopwatch stopWatch = new Stopwatch();

                    stopWatch.Start();
                    Log.InfoFormat("{event} has been called from runtime hub.", _eventName);

                    try
                    {
                        response = await ProcessAsync(request);
                    }
                    catch (Exception ex)
                    {
                        Log.ErrorException("Exceptions occurred during ProcessAsync", ex);

                        // Bad juju, but a TResponse is a Response<TResponseMessage>, so this should be ok.
                        response = (TResponse)DomainModel.Response<TResponseMessage>.Failure(request.Token, ex, 1);
                    }

                    await CompleteAsync(response, stopWatch);
                });
            });
        }

        protected abstract Task<TResponse> ProcessAsync(TRequest request);

        private Task CompleteAsync(TResponse response, Stopwatch stopWatch)
        {
            stopWatch.Stop();
            response.ClientDuration = stopWatch.ElapsedMilliseconds;
            Log.InfoFormat("{service} completed in {milliseconds} ms.", this.GetType().Name, response.ClientDuration);
            Log.InfoFormat("Invoking {remoteMethod} with token {token}.", _remoteMethodName, response.Token);
            return _runtimeHubProxy.Invoke(_remoteMethodName, response);
        }
    }
}
