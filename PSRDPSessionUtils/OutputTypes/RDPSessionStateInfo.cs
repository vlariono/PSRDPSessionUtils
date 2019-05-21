using FreeRDP.Core;

namespace PsRdpSessionUtils.OutputTypes
{
    internal class RdpSessionStateInfo
    {
        private readonly RDP _rdp;
        internal RdpSessionStateInfo(RDP rdp)
        {
            _rdp = rdp;
        }

        public string State => _rdp.Connected ? "Connected" : "Disconnected";

        public string ComputerName { get; internal set; }

        public string UserName { get; internal set; }
    }
}