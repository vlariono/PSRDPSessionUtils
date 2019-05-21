using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Text;

namespace PsRdpSessionUtils.Base
{
    /// <summary>
    /// Base class for RdpSession cmdlets
    /// </summary>
    public abstract class RdpSessionBase:PSCmdlet
    {
        /// <summary>
        /// <para type="description">Computer name</para>
        /// </summary>
        #region Param
        [Parameter(Mandatory = true, ValueFromPipeline = false)]
        [ValidateNotNullOrEmpty()]
        public string ComputerName { get; set; }

        /// <summary>
        /// <para type="description">Credential to connect with</para>
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipeline = false)]
        [ValidateNotNullOrEmpty()]
        public PSCredential Credential { get; set; }
        #endregion
    }
}
