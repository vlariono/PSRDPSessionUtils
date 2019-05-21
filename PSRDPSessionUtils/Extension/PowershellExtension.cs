using System;
using System.Management.Automation;
using PsRdpSessionUtils.Base;

namespace PsRdpSessionUtils.Extension
{
    internal static class PowershellExtension
    {
        public static PowerShell AddCassiaAssembly(this PowerShell powershell, CassiaCmdlet cassiaCmdlet)
        {
            return powershell.AddScript(cassiaCmdlet.CassiaAssemblyImportScript).AddArgument(cassiaCmdlet.CassiaAssemblyBase64);
        }
    }
}
