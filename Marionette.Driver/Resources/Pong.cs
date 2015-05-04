
namespace Marionette.Driver
{
    public class Pong
    {
        public string Nonce { get; set; }
        public int LastSentPayloadIndex { get; set; }
        public int SendPayloadInterval { get; set; }
        public int DurationMilliseconds { get; set; }
        public int PayloadSizeBytes { get; set; }

        public LocationResource Location { get; set; }
    }
}
