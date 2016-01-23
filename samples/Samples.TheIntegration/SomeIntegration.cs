using Samples.ThirdPartyIntegrations;

using System;

namespace Samples.TheIntegration
{
    public class SomeIntegration : IToImplement
    {
        public void WriteMessage(Action<string> tw)
        {
            tw.Invoke(string.Format("Hello from {0}", this.GetType().Name));
        }
    }
}
