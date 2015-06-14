using System.Web.Http;
using Newtonsoft.Json;

namespace Convos
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "MessageAPI",
                routeTemplate: "api/{controller}/{convoId}/{action}/{messageId}",
                defaults: new { messageId = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "ConvoAPI",
                routeTemplate: "api/{controller}/{convoId}",
                defaults: new { action = "convo" }
            );

            config.Formatters.JsonFormatter.SerializerSettings = new JsonSerializerSettings{NullValueHandling = NullValueHandling.Ignore};

        }
    }
}
