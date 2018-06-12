using Store.Repositories.MongoDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Store.Service.Api
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            //repository,service注入
            var unityContainer = UnityContainerConfig.RegisterDependency();
            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(unityContainer);//WebAPi注入
            MongoDBBootstrapper.Bootstrap();//MongoDB 在webapi中使用

        }
    }
}
