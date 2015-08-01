using Nancy;

namespace Drey.Configuration.Modules
{
    public class HomeModule : NancyModule
    {
        public HomeModule()
        {
            Get["/"] = _ => { return "Hello World!"; };
        }
    }
}