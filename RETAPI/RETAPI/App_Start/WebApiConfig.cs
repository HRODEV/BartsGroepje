using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.OData.Builder;
using System.Web.Http.OData.Extensions;
using RETAPI.Models;

namespace RETAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );


            ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
            builder.EntitySet<Station>("Stations");
            builder.EntitySet<Line>("Lines");
            builder.EntitySet<Platform>("Platforms");
            builder.EntitySet<RideStop>("RideStops");
            builder.EntitySet<Ride>("Rides");
            config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
        }
    }
}
