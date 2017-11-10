using SimpleInjector;
using SimpleInjector.Integration.Web;
using SimpleInjector.Integration.Web.Mvc;
using SimpleInjector.Lifestyles;
using softblocks.App_Start;
using softblocks.data.Interface;
using softblocks.data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace softblocks
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            var container = new Container();
            container.Options.DefaultScopedLifestyle = new WebRequestLifestyle();

            // Register your types, for instance using the scoped lifestyle:
            container.Register<IUserRepository, UserRepository>(Lifestyle.Scoped);
            container.Register<IOrganisationRepository, OrganisationRepositoty>(Lifestyle.Scoped);
            container.Register<IAppModuleRepository, AppModuleRepositoty>(Lifestyle.Scoped);
            container.Register<IDocumentTypeRepository, DocumentTypeRepository>(Lifestyle.Scoped);
            container.Register<IModuleMenuRepository, ModuleMenuRepository>(Lifestyle.Scoped);
            container.Register<IDataPanelRepository, DataPanelRepository>(Lifestyle.Scoped);

            container.RegisterMvcControllers(Assembly.GetExecutingAssembly());
            container.Verify();

            DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(container));

            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
