using System.Web.Http;
using AutoMapper;
using Marionette.Driver;
using Owin;
using Swashbuckle.Application;

namespace Marionette
{
    class Startup
    {
        public const string DefaultApiRouteName = "api";

        public void Configuration( IAppBuilder app )
        {
            AutoMapperConfiguration.Configure();

            var config = new HttpConfiguration();

            config.UseJsonSerialization();
            config.Routes.MapHttpRoute( DefaultApiRouteName, "{controller}/{id}", defaults: new { controller = "Root", id = RouteParameter.Optional } );
            config.MapHttpAttributeRoutes();

            config
                .EnableSwagger( c =>
                {
                    c.IncludeXmlComments( "docs.xml" );
                    c.SingleApiVersion( "1.0", "Marionette API" );
                } )
                .EnableSwaggerUi();

            app.UseWebApi( config );
        }
    }

    public static class AutoMapperConfiguration
    {
        public static void Configure()
        {
            Mapper.CreateMap<AppModel, AppResource>();
            Mapper.CreateMap<LocationModel, LocationResource>();
        }
    }
}
