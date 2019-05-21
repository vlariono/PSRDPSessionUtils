using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using System.Reflection;
using System.Text;
using PsRdpSessionUtils.Base;
using PsRdpSessionUtils.Extension;

namespace PsRdpSessionUtils.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">Logs off specified session</para>
    /// <para type="synopsis">This cmdlet logs off user session from the server</para>
    /// </summary>
    [Cmdlet(VerbsCommon.Remove,"RDPSession")]
    public class RemoveRdpSession : CassiaCmdlet
    {
        /// <summary>
        /// <para type="description">Specified sessions will be removed from the server</para>
        /// </summary>
        #region Param
        [Parameter(Mandatory = true,ValueFromPipeline = true)]
        public int[] SessionId { get; set; }
        #endregion

        private readonly string _removeRdpSessionScript;

        /// <inheritdoc />
        public RemoveRdpSession()
        {
            var scriptRoot = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ??
                             throw new InvalidOperationException("Failed to get assembly directory");

            var scriptPath = Path.Combine(scriptRoot, "Scripts", "RemoveRdpSession.ps1");
            _removeRdpSessionScript = File.ReadAllText(scriptPath);
        }

        /// <inheritdoc />
        protected override void ProcessRecord()
        {
            using (var powershell = PowerShell.Create())
            {
                powershell.Runspace = RemoteRunspace;
                powershell.AddCassiaAssembly(this).AddScript(_removeRdpSessionScript).AddArgument(SessionId);
                powershell.Invoke();
            }
        }
    }
}
