using System.Net;

namespace PsRdpSessionUtils.OutputTypes
{
    internal class RdpSessionInfo
    {
        public string ComputerName { get; internal set; }
        public int Id { get; internal set; }
        public string Name { get; internal set; }
        public string UserName { get; internal set; }
        public string State { get; internal set; }
        public string ClientName { get; internal set; }
        public IPAddress ClientIpAddress { get; internal set; }
    }
}