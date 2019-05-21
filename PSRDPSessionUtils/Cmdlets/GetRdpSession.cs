using System;
using System.IO;
using System.Management.Automation;
using System.Net;
using System.Reflection;
using PsRdpSessionUtils.Base;
using PsRdpSessionUtils.Extension;
using PsRdpSessionUtils.OutputTypes;

namespace PsRdpSessionUtils.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">Gets list of available user sessions from the server</para>
    /// <para type="description">The output list will contain sessionId, session state, username, client computer name and ip address</para>
    /// </summary>
    [OutputType(typeof(RdpSessionInfo))]
    [Cmdlet(VerbsCommon.Get, "RDPSession")]
    public sealed class GetRdpSession : CassiaCmdlet
    {
        private readonly string _getRdpSessionScript;

        /// <inheritdoc />
        public GetRdpSession()
        {
            var scriptRoot = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ??
                             throw new InvalidOperationException("Failed to get assembly directory");

            var scriptPath = Path.Combine(scriptRoot, "Scripts", "GetRdpSession.ps1");
            _getRdpSessionScript = File.ReadAllText(scriptPath);
        }

        /// <inheritdoc />
        protected override void ProcessRecord()
        {
            using (var powershell = PowerShell.Create())
            {
                powershell.Runspace = RemoteRunspace;
                powershell.AddCassiaAssembly(this).AddScript(_getRdpSessionScript);
                foreach (var sessionStateInfo in powershell.Invoke())
                {
                    var result = new RdpSessionInfo()
                    {
                        ComputerName = ComputerName,
                        Id = (int)sessionStateInfo.Properties["SessionId"].Value,
                        Name = sessionStateInfo.Properties["WindowStationName"].Value?.ToString(),
                        UserName = sessionStateInfo.Properties["UserAccount"].Value?.ToString(),
                        State = sessionStateInfo.Properties["ConnectionState"].Value?.ToString(),
                        ClientName = sessionStateInfo.Properties["ClientName"].Value?.ToString(),
                        ClientIpAddress = (IPAddress) (sessionStateInfo.Properties["ClientIPAddress"].Value is IPAddress ? sessionStateInfo.Properties["ClientIPAddress"].Value : IPAddress.None)
                    };

                    WriteObject(result);
                }
            }
        }
    }
}