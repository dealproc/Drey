using Drey.Logging;
using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Lifetime;
using System.Security.Permissions;

namespace Drey
{
    /// <summary>
    /// Manages lifetimes of any MarshalByRef objects in the system.
    /// <remarks>Pulled from: http://www.brad-smith.info/blog/archives/500 </remarks>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="System.Runtime.Remoting.Lifetime.ISponsor" />
    /// <seealso cref="System.IDisposable" />
    [Serializable]
    [SecurityPermission(SecurityAction.Demand, Infrastructure = true)]
    public sealed class Sponsor<T> : ISponsor, IDisposable where T : class
    {
        static ILog _log = LogProvider.For<Sponsor<T>>();

        T _protege;

        bool _disposed = false;


        /// <summary>
        /// Gets or sets the protege.
        /// </summary>
        /// <value>
        /// The protege.
        /// </value>
        /// <exception cref="System.ObjectDisposedException">Protege</exception>
        public T Protege
        {
            get
            {
                if (_disposed) { throw new ObjectDisposedException("Protege"); }

                return _protege;
            }
            set
            {
                _protege = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Sponsor{T}"/> class.
        /// </summary>
        /// <param name="protege">The protege.</param>
        public Sponsor(T protege)
        {
            Protege = protege;

            if (Protege is MarshalByRefObject)
            {
                object lifetimeService = RemotingServices.GetLifetimeService((MarshalByRefObject)(object)Protege);
                if (lifetimeService is ILease)
                {
                    ILease lease = (ILease)lifetimeService;
                    lease.Register(this);
                }
            }
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="Sponsor{T}"/> class.
        /// </summary>
        ~Sponsor()
        {
            Dispose(false);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Requests a sponsoring client to renew the lease for the specified object.
        /// </summary>
        /// <param name="lease">The lifetime lease of the object that requires lease renewal.</param>
        /// <returns>
        /// The additional lease time for the specified object.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="Infrastructure" />
        /// </PermissionSet>
        public TimeSpan Renewal(ILease lease)
        {
            _log.TraceFormat("Renewing lease for {object}.", GetType().FullName);
            if (_disposed)
            {
                return TimeSpan.Zero;
            }

            return LifetimeServices.RenewOnCallTime;
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        private void Dispose(bool disposing)
        {
            if (!disposing || _disposed) { return; }

            if (Protege is IDisposable) ((IDisposable)Protege).Dispose();

            if (Protege is MarshalByRefObject)
            {
                object lifetimeService = RemotingServices.GetLifetimeService((MarshalByRefObject)(object)Protege);
                if (lifetimeService is ILease)
                {
                    ILease lease = (ILease)lifetimeService;
                    lease.Unregister(this);
                }
            }

            Protege = null;
            _disposed = true;
        }
    }
}
