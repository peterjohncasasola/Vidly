using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Http;
using AutoMapper;
using HibernatingRhinos.Profiler.Appender.EntityFramework;
using Vidly.Models.DTO;

namespace Vidly
{
  public class MvcApplication : System.Web.HttpApplication
  {
    protected void Application_Start()
    {
      Mapper.Initialize(m => m.AddProfile<MappingProfile>());
      var config = GlobalConfiguration.Configuration;
      config.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
      GlobalConfiguration.Configure(WebApiConfig.Register);
      AreaRegistration.RegisterAllAreas();
      FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
      RouteConfig.RegisterRoutes(RouteTable.Routes);
      BundleConfig.RegisterBundles(BundleTable.Bundles);
      EntityFrameworkProfiler.Initialize();
      
    }
  }
}