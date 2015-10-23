using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

// Influenced by the Caliburn Micro event aggregator.
namespace Drey.Server
{
    public interface IEventBus
    {
        void Subscribe(object subscriber);
        void Subscribe(object subscriber, object token);
        void Unsubscribe(object subscriber);
        void Publish(object message);
        void Publish(object message, object token);
    }

    public class EventBus : IEventBus
    {
        readonly List<Handler> handlers = new List<Handler>();
        public static Action<object, object> HandlerResultProcessing = (target, result) => { };

        public void Subscribe(object subscriber)
        {
            Subscribe(subscriber, null);
        }

        public void Subscribe(object subscriber, object token)
        {
            if (subscriber == null)
            {
                throw new ArgumentNullException("subscriber");
            }

            lock (handlers)
            {
                if (handlers.Any(x => x.Matches(subscriber)))
                {
                    return;
                }
                handlers.Add(new Handler(subscriber, token));
            }
        }

        public void Unsubscribe(object subscriber)
        {
            if (subscriber == null)
            {
                throw new ArgumentNullException("subscriber");
            }
            lock (handlers)
            {
                var found = handlers.FirstOrDefault(x => x.Matches(subscriber));
                if (found != null)
                {
                    handlers.Remove(found);
                }
            }
        }

        public void Publish(object message)
        {
            Publish(message, null);
        }
        public void Publish(object message, object token)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            Handler[] toNotify;
            lock (handlers)
            {
                toNotify = handlers.ToArray();
            }

            var messageType = message.GetType();

            var dead = toNotify
                .Where(handler => !handler.Handle(messageType, message, token))
                .ToArray();

            if (dead.Any())
            {
                lock (handlers)
                {
                    dead.Select(x => handlers.Remove(x));
                }
            }
        }


        class Handler
        {
            readonly WeakReference _reference;
            readonly Dictionary<Type, MethodInfo> _supportedHandlers = new Dictionary<Type, MethodInfo>();
            readonly object _token;

            public bool IsDead
            {
                get { return _reference.Target == null; }
            }

            public Handler(object handler, object token)
            {
                _reference = new WeakReference(handler);
                _token = token;

                var interfaces = handler.GetType().GetInterfaces()
                    .Where(x => typeof(IHandle).IsAssignableFrom(x) && x.IsGenericType);

                foreach (var @interface in interfaces)
                {
                    var type = @interface.GetGenericArguments()[0];
                    var method = @interface.GetMethod("Handle", new Type[] { type });
                    _supportedHandlers[type] = method;
                }
            }

            public bool Matches(object instance)
            {
                return _reference.Target == instance;
            }

            public bool Handle(Type messageType, object message, object token)
            {
                var target = _reference.Target;
                if (target == null)
                {
                    return false;
                }

                foreach (var pair in _supportedHandlers)
                {
                    var isAssignable = pair.Key.IsAssignableFrom(messageType);

                    if (
                        (isAssignable && _token == null) ||
                        (isAssignable && _token.Equals(token))
                       )
                    {
                        var result = pair.Value.Invoke(target, new[] { message });
                        if (result != null)
                        {
                            HandlerResultProcessing(target, result);
                        }
                    }
                }

                return true;
            }

            public bool Handles(Type messageType)
            {
                return _supportedHandlers.Any(pair => pair.Key.IsAssignableFrom(messageType));
            }
        }
    }

    /// <summary>
    /// Locator interface for finding IHandle{T} instances.
    /// <remarks>
    /// DO NOT USE THIS!!! implement one of the generic IHandle<> interfaces 
    /// </remarks>
    /// </summary>
    public interface IHandle { }

    /// <summary>
    /// Expresses a handler or subscriber of an event that handles all messages of a given
    /// type transmitted on the event bus.
    /// </summary>
    /// <typeparam name="TMessage">The type of the message.</typeparam>
    public interface IHandle<TMessage> : IHandle
    {
        void Handle(TMessage message);
    }

    /// <summary>
    /// Expresses a handler or subscriber of an event, where they can decide whether or not
    /// to handle the event based on a provided filter.
    /// </summary>
    /// <typeparam name="TMessage">The type of the message.</typeparam>
    public interface IHandleWithFilter<TMessage> : IHandle
    {
        bool Filter(TMessage message);
        void Handle(TMessage message);
    }
}