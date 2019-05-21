using System;
using System.Management.Automation;
using FreeRDP.Core;
using PsRdpSessionUtils.Base;
using PsRdpSessionUtils.OutputTypes;

namespace PsRdpSessionUtils.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">Creates new rdp session</para>
    /// <para type="description">The cmdlet create new RDP session to the server</para>
    /// <para type="description">The cmdlet does not show any output screen</para>
    /// <para type="description">The session created by this cmdlet can be used with psexec to start commands in the user's session</para>
    /// </summary>
    [OutputType(typeof(RdpSessionStateInfo))]
    [Cmdlet (VerbsCommon.New, "RDPSession")]
    public sealed class NewRdpSession : RdpSessionBase
    {
        /// <inheritdoc />
        protected override void ProcessRecord ()
        {
            var networkCredential = Credential.GetNetworkCredential ();
            var userName = networkCredential.UserName;
            var userDomain = networkCredential.Domain;
            var userPassword = networkCredential.Password;

            var rdp = new RDP ();
            rdp.Connect (ComputerName, userDomain, userName, userPassword);

            if (rdp.Connected)
            {
                var sessionState = new RdpSessionStateInfo (rdp)
                {
                    ComputerName = ComputerName,
                    UserName = userDomain.Length > 0? $"{userDomain.Split('.')[0]}\\{userName}": userName
                };
                WriteObject (sessionState);
            }
            else
            {
                WriteError (
                    new ErrorRecord (
                        new InvalidOperationException ($"Failed to connect to {ComputerName}"),
                        null,
                        ErrorCategory.ConnectionError, ComputerName));
            }
        }
    }
}