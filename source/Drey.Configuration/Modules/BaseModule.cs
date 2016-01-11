using Drey.Configuration.Services;
using Drey.Logging;

using Nancy;
using Nancy.Extensions;

using System;
using System.Threading.Tasks;

namespace Drey.Configuration.Modules
{
    /// <summary>
    /// Base module, which provides common business logic across all nancy modules.
    /// </summary>
    public abstract class BaseModule : NancyModule
    {
        static readonly ILog _log = LogProvider.For<BaseModule>();

        protected IEventBus EventBus { get; private set; }
        protected IGlobalSettingsService GlobalSettingsService { get; private set; }

        public BaseModule(IEventBus eventBus, Services.IGlobalSettingsService globalSettingsService, bool verifyConfigured)
            : base()
        {
            EventBus = eventBus;
            GlobalSettingsService = globalSettingsService;

            WireSetupPipeline(verifyConfigured);
        }

        public BaseModule(IEventBus eventBus, Services.IGlobalSettingsService globalSettingsService, string modulePath, bool verifyConfigured) : base(modulePath)
        {
            EventBus = eventBus;
            GlobalSettingsService = globalSettingsService;

            WireSetupPipeline(verifyConfigured);
        }

        /// <summary>
        /// Enforces that the system must have proper settings before continuing to use the rest of the console.
        /// </summary>
        /// <param name="verifyConfigured">if set to <c>true</c> [verify configured].</param>
        private void WireSetupPipeline(bool verifyConfigured)
        {
            if (!verifyConfigured) { return; }

            this.Before.AddItemToEndOfPipeline(context =>
            {
                var isNotSetupPath = !context.Request.Path.Equals("/Setup");
                var globalSettingsAreNotValid = !GlobalSettingsService.HasValidSettings();
                if (isNotSetupPath && globalSettingsAreNotValid)
                {
                    return context.GetRedirect("~/Setup");
                }

                return (Response)null;
            });
        }

        /// <summary>
        /// Issues the RecycleApp event.
        /// </summary>
        /// <returns></returns>
        protected dynamic RestartAppDomains()
        {
            Task.Factory.StartNew(() =>
            {
                _log.Debug("Waiting one second");
                Task.Delay(TimeSpan.FromSeconds(1)).Wait();
                _log.Debug("Issuing recycle app command.");

                EventBus.Publish(new Infrastructure.Events.RecycleApp());
            });

            return Response.AsRedirect("~/pending", Nancy.Responses.RedirectResponse.RedirectType.SeeOther);
        }
    }
}