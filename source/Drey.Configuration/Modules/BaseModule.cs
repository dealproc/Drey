using Drey.Configuration.Services;
using Drey.Logging;

using Nancy;
using Nancy.Extensions;

using System;
using System.Threading.Tasks;

namespace Drey.Configuration.Modules
{
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