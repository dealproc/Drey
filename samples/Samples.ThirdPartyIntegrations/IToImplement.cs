using System;

namespace Samples.ThirdPartyIntegrations
{
    public interface IToImplement
    {
        void WriteMessage(Action<string> tw);
    }

    public class CoreWriter : IToImplement
    {
        public void WriteMessage(Action<string> tw)
        {
            tw.Invoke(string.Format("Hello from {0}", this.GetType().Name));
        }
    }
}
