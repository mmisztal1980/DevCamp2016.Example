using Microsoft.Owin;
using Owin;

// http://www.asp.net/aspnet/overview/owin-and-katana/owin-startup-class-detection
[assembly: OwinStartup("IIS-Startup", typeof(Example.Api.IISStartup))]

namespace Example.Api
{
    public class IISStartup
    {
        public void Configuration(IAppBuilder app)
        {
            app
                .UseWebApi()
                .Use(async (ctx, next) =>
                {
                    if (ctx.Request.Path.Value.Equals("/"))
                    {
                        ctx.Response.Redirect("swagger/docs/v1");
                        return;
                    }

                    await next();
                });
            // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888
        }
    }
}
