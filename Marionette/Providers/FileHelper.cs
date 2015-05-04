using System.Net.Http;
using System.Threading.Tasks;

namespace Marionette
{
    public class FileHelper
    {
        HttpContent _content;

        public FileHelper( HttpRequestMessage request )
        {
            _content = request.Content;
        }

        public async Task SaveAsync( string directoryPath, string fileName )
        {
            var streamProvider = new SaveLocalFileStreamProvider( directoryPath, fileName );
            await _content.ReadAsMultipartAsync( streamProvider );
        }
    }
}
