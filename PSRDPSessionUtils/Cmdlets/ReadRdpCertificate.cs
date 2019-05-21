using System.Management.Automation;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;

namespace PsRdpSessionUtils.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">Gets certificate provided by the server</para>
    /// <para type="description">The cmdlet returns x509 certificate used by the server</para>
    /// </summary>
    [OutputType(typeof(X509Certificate2))]
    [Cmdlet(VerbsCommunications.Read, "RDPCertificate")]
    public sealed class ReadRdpCertificate : PSCmdlet
    {
        /// <summary>
        /// <para type="description">Computer name</para>
        /// </summary>
        #region Param
        [Parameter(Mandatory = true, ValueFromPipeline = false)]
        [ValidateNotNullOrEmpty()]
        public string ComputerName { get; set; }

        /// <summary>
        /// <para type="description">Port to use, default is 3389</para>
        /// </summary>
        [Parameter(Mandatory = false, ValueFromPipeline = false)]
        public int Port { get; set; } = 3389;
        #endregion

        /// <inheritdoc />
        protected override void ProcessRecord()
        {
            using (var tcpClient = new TcpClient(ComputerName, Port))
            using (var tcpStream = tcpClient.GetStream())
            using (var sslStream = new SslStream(tcpStream, false, (sender, certificate, chain, errors) => true))
            {
                sslStream.AuthenticateAsClient(ComputerName);
                if (sslStream.RemoteCertificate != null)
                {
                    var remoteCertificate = new X509Certificate2(sslStream.RemoteCertificate);
                    WriteObject(remoteCertificate);
                }
            }
        }
    }
}
