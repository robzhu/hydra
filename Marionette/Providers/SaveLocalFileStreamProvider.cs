﻿using System.Net.Http;
using System.Net.Http.Headers;

namespace Marionette
{
    public class SaveLocalFileStreamProvider : MultipartFormDataStreamProvider
    {
        private string _fileName;

        public SaveLocalFileStreamProvider( string directory, string fileName )
            : base( directory )
        {
            _fileName = fileName;
        }

        public override string GetLocalFileName( HttpContentHeaders headers )
        {
            return _fileName;
        }
    }
}
