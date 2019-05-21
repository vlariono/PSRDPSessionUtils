using System;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Remoting;
using System.Management.Automation.Runspaces;
using System.Reflection;

namespace PsRdpSessionUtils.Base
{
    /// <summary>
    /// This is base class for all cmdlet that are using Cassia assembly
    /// </summary>
    public abstract class CassiaCmdlet : RdpSessionBase,IDisposable
    {
        // runspace init should be done when all properties are set
        private readonly Lazy<Runspace> _remoteRunspace;

        /// <inheritdoc />
        protected CassiaCmdlet()
        {
            _remoteRunspace = new Lazy<Runspace>(() =>
            {
                var wsmanConnectionInfo = new WSManConnectionInfo
                {
                    ComputerName = ComputerName,
                    Credential = Credential
                };

                wsmanConnectionInfo.SetSessionOptions(new PSSessionOption()
                {
                    IncludePortInSPN = true
                });

                var remoteRunspace = RunspaceFactory.CreateRunspace(wsmanConnectionInfo);

                remoteRunspace.Open();

                return remoteRunspace;
            });

            var scriptRoot = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ??
                             throw new InvalidOperationException("Failed to get assembly directory");

            var cassiaAssemblyPath = Path.Combine(scriptRoot, "Lib", "Cassia.dll");
            CassiaAssemblyBase64 = Convert.ToBase64String(File.ReadAllBytes(cassiaAssemblyPath));

            var scriptPath = Path.Combine(scriptRoot, "Scripts", "CassiaCmdlet.ps1");
            CassiaAssemblyImportScript = File.ReadAllText(scriptPath);
        }

        /// <summary>
        /// Remote runspace to run commands in, the runspace is lazy initialized
        /// </summary>
        protected Runspace RemoteRunspace => _remoteRunspace.Value;

        /// <summary>
        /// This is base64 representation of Cassia assembly, convert it from base64 to get original bytes.
        /// </summary>
        public string CassiaAssemblyBase64 { get; }

        /// <summary>
        /// This script is used to load the assembly in remote session
        /// </summary>
        public string CassiaAssemblyImportScript { get; }

        /// <inheritdoc />
        public void Dispose()
        {
            RemoteRunspace?.Dispose();
        }
    }
}