using System;
using System.Linq;
using System.Security.Cryptography;

namespace Hydra.Shared
{
    public class Payload
    {
        private static Random _rand = new Random();
        private static HashAlgorithm _hashProvider = new SHA1CryptoServiceProvider();

        public int Sequence { get; set; }
        public byte[] Data { get; set; }
        public byte[] Hash { get; set; }

        public Payload() { }
        public Payload( int sequence, int sizeBytes )
        {
            Sequence = sequence;
            Data = new byte[ sizeBytes ];
            _rand.NextBytes( Data );
            Hash = _hashProvider.ComputeHash( Data );
        }

        public bool IsDataValid()
        {
            return Hash.SequenceEqual( _hashProvider.ComputeHash( Data ) );
        }
    }
}
