using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Drey.Configuration
{
    /// <summary>
    /// Provides a framework to broker messages within an application, keeping individual components loosely coupled.
    /// TODO: Consider replacing with MediatR or another library.
    /// </summary>
    public interface IEventBus
    {
        /// <summary>
        /// Subscribes the subscriber to the bus, to listen for events.
        /// </summary>
        /// <param name="subscriber">The subscriber.</param>
        void Subscribe(object subscriber);

        /// <summary>
        /// Subscribes the subscriber to the bus, to start listening for messages, with a token for filtering.
        /// </summary>
        /// <param name="subscriber">The subscriber.</param>
        /// <param name="token">The token.</param>
        void Subscribe(object subscriber, object token);
        
        /// <summary>
        /// Unsubscribes the specified subscriber.
        /// </summary>
        /// <param name="subscriber">The subscriber.</param>
        void Unsubscribe(object subscriber);
        
        /// <summary>
        /// Publishes a message to all subscribers.
        /// </summary>
        /// <param name="message">The message.</param>
        void Publish(object message);
        
        /// <summary>
        /// Publishes a message to all subscribers, with an ability to filter which subscribers receive the message by the token value.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="token">The token.</param>
        void Publish(object message, object token);
    }

    /// <summary>
    /// 
    /// </summary>
    public class EventBus : IEventBus
    {
        readonly List<Handler> handlers = new List<Handler>();

        /// <summary>
        /// The handler result processing
        /// </summary>
        public static Action<object, object> HandlerResultProcessing = (target, result) => { };

        /// <summary>
        /// Subscribes the subscriber to the bus, to listen for events.
        /// </summary>
        /// <param name="subscriber">The subscriber.</param>
        public void Subscribe(object subscriber)
        {
            Subscribe(subscriber, null);
        }

        /// <summary>
        /// Subscribes the subscriber to the bus, to start listening for messages, with a token for filtering.
        /// </summary>
        /// <param name="subscriber">The subscriber.</param>
        /// <param name="token">The token.</param>
        /// <exception cref="System.ArgumentNullException">subscriber</exception>
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

        /// <summary>
        /// Unsubscribes the specified subscriber.
        /// </summary>
        /// <param name="subscriber">The subscriber.</param>
        /// <exception cref="System.ArgumentNullException">subscriber</exception>
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

        /// <summary>
        /// Publishes a message to all subscribers.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Publish(object message)
        {
            Publish(message, null);
        }

        /// <summary>
        /// Publishes a message to all subscribers, with an ability to filter which subscribers receive the message by the token value.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="token">The token.</param>
        /// <exception cref="System.ArgumentNullException">message</exception>
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
                .ToList();

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
}