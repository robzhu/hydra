using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;
using Rz.Http;
using Marionette.Driver;

namespace Marionette
{
    public class ProcessController : ApiController
    {
        public const string Prefix = "Process";
        public const string Route_GetProcessById = "GetById";

        private AppProvider Provider { get; set; }
        private string ApiRoute { get; set; }

        public ProcessController()
        {
            Provider = AppProvider.Instance;
            ApiRoute = Startup.DefaultApiRouteName;
        }

        private string BuildLink( object routeValues )
        {
            return Url.Link( ApiRoute, routeValues );
        }

        /// <summary>
        /// Creates a new instance of the app with the specified App Id.
        /// </summary>
        /// <param name="appId">The Id of the app to spawn a new instance of.</param>
        [ResponseType( typeof( ProcessResource ) )]
        public IHttpActionResult Post( string appId, string parameters = "" )
        {
            try
            {
                var process = Provider.LaunchInstance( appId, parameters );
                var resource = Convert( process );
                return ResponseMessage( this.CreatedAtSelfLocation( resource ) );
            }
            catch( Exception ex )
            {
                return BadRequest( ex.Message );
            }
        }

        /// <summary>
        /// Gets a process by id.
        /// </summary>
        /// <param name="id">The id of the process to get.</param>
        //[Route( Prefix + "/{id?}", Name = Route_GetProcessById )]
        [ResponseType( typeof( ProcessResource ) )]
        public IHttpActionResult Get( int id, string expand = null )
        {
            var process = Provider.GetProcessById( id );
            if( process == null )
            {
                return NotFound();
            }

            return Ok( Convert( process, expand ) );
        }

        /// <summary>
        /// Gets all running processes.
        /// </summary>
        [HttpGet]
        [ResponseType( typeof( IEnumerable<ProcessResource> ) )]
        public IHttpActionResult GetAll()
        {
            List<ProcessResource> resources = new List<ProcessResource>();
            foreach( var process in Provider.GetAllProcesses() )
            {
                resources.Add( Convert( process ) );
            }

            return Ok( resources );
        }

        /// <summary>
        /// Deletes the process by id.
        /// </summary>
        /// <param name="id">The id of the process to delete.</param>
        public IHttpActionResult Delete( int id )
        {
            var process = Provider.GetProcessById( id );
            if( process == null )
            {
                return NotFound();
            }

            Provider.DeleteProcessById( id );
            return Ok();
        }

        private AppResource ConvertAppResource( AppModel model )
        {
            var resource = new AppResource
            {
                Id = model.Id,
                ExeFileName = model.ExeFileName
            };
            resource.Href = BuildLink( new { controller = AppController.Prefix, id = model.Id } );
            return resource;
        }

        private ProcessResource Convert( ProcessModel model, string expand = null )
        {
            var resource = new ProcessResource();

            resource.ProcessId = model.ProcessId;
            resource.RunParameters = model.RunParameters;
            resource.Href = BuildLink( new { id = model.ProcessId } );
            resource.App = BuildLink( new { controller = AppController.Prefix, id = model.AppId } );

            if( expand != null )
            {
                string[] expandArgs = expand.Split( ',' );
                if( expandArgs.Any( arg => arg.ToLowerInvariant() == "app" ) )
                {
                    var appModel = Provider.GetAppById( model.AppId );
                    resource.App = ConvertAppResource( appModel );
                }
            }

            return resource;
        }
    }
}
