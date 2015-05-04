using System;
using Hydra.Shared;
using Rz.Http;

namespace Marionette.Driver
{
    public class AvailabilityStats
    {
        public int DisconnectCount { get; set; }
        public int PingFailures { get; set; }
        public int ConnectionEstablishmentFailCount { get; set; }
        public DateTime LastPayloadReceived { get; set; }
        public int TimeSinceLastPayloadReceived { get; set; }
    }

    public class IntegrityStats
    {
        public int LastServerPayloadIndex { get; set; }

        public long PayloadSequence { get; set; }
        public long PayloadCount { get; set; }
        public int PayloadDataErrors { get; set; }
        public int PayloadSequenceErrors { get; set; }
        public long TotalBytesReceived { get; set; }
    }

    public class ClientStatsResource : Resource
    {
        public Hyperlink<ProcessResource> Process { get; set; }

        public int Id { get; set; }
        public string Name { get; set; }
        public string ConnectionState { get; set; }
        public string Transport { get; set; }
        public DateTime Timestamp { get; set; }

        public Statistic Ping { get; set; }
        public Statistic PayloadReceiveInterval { get; set; }
        public int ServerSendPayloadInterval { get; set; }
        public int ServerPayloadSizeKiloBytes { get; set; }
        public LocationResource ServerLocation { get; set; }
        public LocationResource ClientLocation { get; set; }

        public AvailabilityStats Availability { get; set; }
        public IntegrityStats DataIntegrity { get; set; }
    }
}
