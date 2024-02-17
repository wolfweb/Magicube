using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Security.Authentication;

namespace Magicube.MessageService.RabbitMQ.EndPoints {
    public class RabbitConnectionConfig {
        public SslProtocols?   AmqpUriSslProtocols          { get; set; }
                               
        public bool?           AutomaticRecoveryEnabled     { get; set; } = true;
                               
        public string          HostName                     { get; set; }
                               
        public TimeSpan?       NetworkRecoveryInterval      { get; set; }
                               
        public TimeSpan?       HandshakeContinuationTimeout { get; set; }
                               
        public TimeSpan?       ContinuationTimeout          { get; set; }
                               
        public int?            Port                         { get; set; }
                               
        public TimeSpan?       RequestedConnectionTimeout   { get; set; }
                               
        public TimeSpan?       SocketReadTimeout            { get; set; }
                               
        public TimeSpan?       SocketWriteTimeout           { get; set; }

        public RabbitSslOption Ssl                          { get; set; } = new();

        public bool?           TopologyRecoveryEnabled      { get; set; }
                               
        public string          Password                     { get; set; }
                               
        public ushort?         RequestedChannelMax          { get; set; }
                               
        public uint?           RequestedFrameMax            { get; set; }

        public TimeSpan?       RequestedHeartbeat           { get; set; }
                               
        public bool?           UseBackgroundThreadsForIO    { get; set; }
                               
        public string          UserName                     { get; set; }
                               
        public string          VirtualHost                  { get; set; }
                               
        public string          ClientProvidedName           { get; set; }

        public IDictionary<string, object> ClientProperties { get; set; } = new Dictionary<string, object>();
    }

    public sealed class RabbitSslOption : IEquatable<RabbitSslOption> {
        public SslPolicyErrors? AcceptablePolicyErrors     { get; set; }

        public string           CertPassphrase             { get; set; }
                                
        public string           CertPath                   { get; set; }
                                
        public bool             CheckCertificateRevocation { get; set; }
                                
        public bool             Enabled                    { get; set; }
                                
        public string           ServerName                 { get; set; }
                                
        public SslProtocols     Version                    { get; set; }

        public bool Equals(RabbitSslOption other) {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return AcceptablePolicyErrors == other.AcceptablePolicyErrors &&
                   string.Equals(CertPassphrase, other.CertPassphrase, StringComparison.Ordinal) &&
                   string.Equals(CertPath, other.CertPath, StringComparison.Ordinal) &&
                   CheckCertificateRevocation == other.CheckCertificateRevocation && Enabled == other.Enabled &&
                   string.Equals(ServerName, other.ServerName, StringComparison.Ordinal) &&
                   Version == other.Version;
        }
    }
}
