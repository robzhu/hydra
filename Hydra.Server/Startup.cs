using System.Web.Http;
using Owin;
using Swashbuckle.Application;

namespace Hydra.Server
{
    public class Startup
    {
        public const string DefaultApiRouteName = "api";

        public void Configuration( IAppBuilder app )
        {
            var config = new HttpConfiguration();

            config.UseJsonSerialization();
            config.Routes.MapHttpRoute( "health", "health", defaults: new { controller = "Health" } );
            config.Routes.MapHttpRoute( DefaultApiRouteName, "api/{controller}/{id}", defaults: new { controller = "Root", id = RouteParameter.Optional } );
            //config.Routes.MapHttpRoute( DefaultApiRouteName, "api/{controller}/{id}", defaults: new { id = RouteParameter.Optional } );
            config.MapHttpAttributeRoutes();

            config.EnableSwagger( c =>
                {
                    c.IncludeXmlComments( "docs.xml" );
                    c.SingleApiVersion( "0.1", "Hydra.Server API" );
                } )
                .EnableSwaggerUi();

            app.UseWebApi( config );

            app.MapSignalR();
        }
    }
}
