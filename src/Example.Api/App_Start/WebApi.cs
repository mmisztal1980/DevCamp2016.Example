using System.Net.Http.Headers;
using System.Web.Http;
using Owin;
using Swashbuckle.Application;

namespace Example.Api
{
    public static class WebApi
    {
        public static IAppBuilder UseWebApi(this IAppBuilder app)
        {
            var configuration = new HttpConfiguration();
            configuration.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional });

            // Enable Json response 
            configuration.Formatters
                .JsonFormatter
                .SupportedMediaTypes
                .Add(new MediaTypeHeaderValue("text/html"));

            configuration
                .EnableSwagger(c => c.SingleApiVersion("v1", "Example DevCamp2016 API"))
                .EnableSwaggerUi("swagger/docs/{apiVersion}");

            app.UseWebApi(configuration);

            return app;
        }
    }
}