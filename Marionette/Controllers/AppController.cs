using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using AutoMapper;
using Marionette.Driver;
using Rz.Http;

namespace Marionette
{
    [RoutePrefix( AppController.Prefix )]
    public class AppController : ApiController
    {
        public const string Prefix = "app";
        public const string Route_GetAllApps = "GetAllApps";
        public const string Route_GetProcessesByAppId = "GetProcessesByAppId";

        private AppProvider Provider { get; set; }
        private string ApiRoute { get; set; }

        public AppController()
        {
            ApiRoute = Startup.DefaultApiRouteName;
            Provider = AppProvider.Instance;
        }

        private string BuildLink( object routeValues )
        {
            return Url.Link( ApiRoute, routeValues );
        }

        /// <summary>
        /// Gets the app with the specified id.
        /// </summary>
        /// <param name="id">the id of the app.</param>
        /// <param name="expand">the linked resources to expand, separated by commas. Ex: "?exapnd=processes".</param>
        [ResponseType( typeof( AppResource ) )]
        public IHttpActionResult Get( string id, string expand = null )
        {
            var app = Provider.GetAppById( id );
            if( app == null )
            {
                return NotFound();
            }
            else
            {
                return Ok( ConvertAppModelToResource( app, expand ) );
            }
        }

        /// <summary>
        /// Gets all the apps
        /// </summary>
        [ResponseType( typeof( ResourceCollection<AppResource> ) )]
        public IHttpActionResult Get( string expand = null )
        {
            List<AppResource> resources = new List<AppResource>();
            foreach( var app in Provider.GetAll() )
            {
                resources.Add( ConvertAppModelToResource( app, expand ) );
            }

            var collectionResource = new ResourceCollection<AppResource>()
            {
                Href = Request.RequestUri.ToString(),
                Items = resources,
            };
            return Ok( collectionResource );
        }

        /// <summary>
        /// Gets all the current processes with the specified App Id
        /// </summary>
        /// <param name="appId">the id of the app.</param>
        [ResponseType( typeof( ResourceCollection<ProcessResource> ) )]
        [Route( "{appId}/processes", Name = Route_GetProcessesByAppId )]
        public IHttpActionResult GetProcessesByAppId( string appId )
        {
            List<ProcessResource> resources = new List<ProcessResource>();
            foreach( var p in Provider.GetProcessesByAppId( appId ) )
            {
                resources.Add( new ProcessResource
                {
                    ProcessId = p.ProcessId,
                    RunParameters = p.RunParameters,
                    App = BuildLink( new { controller = Prefix, id = appId } ),
                    Href = BuildLink( new { controller = ProcessController.Prefix, id = p.ProcessId } )
                } );
            }

            var link = Url.Link( Route_GetProcessesByAppId, new { appId = appId } );
            var collectionResource = new ResourceCollection<ProcessResource>()
            {
                Href = link,
                Items = resources,
            };
            return Ok( collectionResource );
        }

        /// <summary>
        /// Uploads a portable application archive for the app with the specified Id.
        /// </summary>
        public async Task<IHttpActionResult> CreateApp( string id )
        {
            try
            {
                if( !Request.Content.IsMimeMultipartContent( "form-data" ) )
                {
                    //throw new HttpResponseException( Request.CreateResponse( HttpStatusCode.UnsupportedMediaType ) );
                    return BadRequest( "the request must specify ContentType as multipart/form-data" );
                }

                await Provider.CreateAppAsync( id, new FileHelper( Request ) );
                return Ok();
            }
            catch( Exception ex )
            {
                return BadRequest( ex.Message );
            }
        }

        /// <summary>
        /// Deletes an app with the specified Id.
        /// </summary>
        /// <param name="id">The id of the app to delete.</param>
        public IHttpActionResult DeleteApp( string id )
        {
            try
            {
                Provider.DeleteAppById( id );
                return Ok();
            }
            catch( Exception ex )
            {
                return BadRequest( ex.Message );
            }
        }

        private AppResource ConvertAppModelToResource( AppModel model, string expand = null )
        {
            var resource = Mapper.Map<AppResource>( model );
            resource.Href = BuildLink( new { id = model.Id } );

            resource.LaunchInstance = new Hyperlink( BuildLink( new { controller = ProcessController.Prefix, appId = model.Id } ) );

            var href_GetProcessesByAppId = Url.Link( Route_GetProcessesByAppId, new { appId = model.Id } );
            resource.Processes = href_GetProcessesByAppId;

            var eq = new ExpandQuery( expand );
            if( eq.Contains( "processes" ) )
            {
                IEnumerable<ProcessModel> processes = Provider.GetProcessesByAppId( model.Id );

                var collection = new List<ProcessResource>();
                foreach( var processModel in processes )
                {
                    collection.Add( ConvertProcessModelToResource( processModel ) );
                }

                resource.Processes = new ResourceCollection<ProcessResource>
                {
                    Href = href_GetProcessesByAppId,
                    Items = collection,
                };
            }

            return resource;
        }

        private ProcessResource ConvertProcessModelToResource( ProcessModel model )
        {
            var resource = new ProcessResource();

            resource.ProcessId = model.ProcessId;
            resource.RunParameters = model.RunParameters;

            resource.Href = BuildLink( new { controller = ProcessController.Prefix, id = model.ProcessId } );
            resource.App = BuildLink( new { controller = AppController.Prefix, id = model.AppId } );

            return resource;
        }
    }
}
