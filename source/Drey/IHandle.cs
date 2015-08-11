namespace Drey
{
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