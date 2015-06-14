using Convos.Managers;
using Convos.Controllers;
using Convos.Utilities;
using Microsoft.Practices.Unity;
using System.Web.Http;
using Unity.WebApi;

namespace Convos
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();
            
            // register all your components with the container here
            // it is NOT necessary to register your controllers
            
            container.RegisterType<IConvoManager, ConvoManager>();
            container.RegisterType<ISQLPlatform, SQLServerPlatform>();
            container.RegisterType<ILogger, Logger>();
            container.RegisterInstance<IConvoContract>(
                new ServerCacheDecorator(
                    new ConvoContract(
                        new LoggingDecorator(
                            new ConvoManager(
                                    container.Resolve<ISQLPlatform>()), 
                                    container.Resolve<ILogger>())), 
                    new ServerCache()));
            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}