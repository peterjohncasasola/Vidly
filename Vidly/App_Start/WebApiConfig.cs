using System.Web.Http;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Vidly.Models;

namespace Vidly
{
    public static class WebApiConfig
    {
    
        public static void Register(HttpConfiguration config)
        {
          var settings = config.Formatters.JsonFormatter.SerializerSettings;
          settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
          settings.Formatting = Formatting.Indented;


             config.MapHttpAttributeRoutes();
       
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );


            //Odata
            /*
            ODataModelBuilder builder = new ODataConventionModelBuilder();
            
            config.Count().Filter().OrderBy().Expand().Select().MaxTop(null);
            builder.EntitySet<Rental>("Rentals");
            builder.EntitySet<Customer>("Customers");
            builder.EntitySet<RentalDetail>("RentalDetails");
            builder.EntitySet<MembershipType>("MembershipTypes");
            builder.EntitySet<Movie>("Movies");
            config.MapODataServiceRoute(
              routeName: "ODataApiRoute",
              routePrefix: "odata/api",
              model: builder.GetEdmModel());
            */
        }
    }
}
