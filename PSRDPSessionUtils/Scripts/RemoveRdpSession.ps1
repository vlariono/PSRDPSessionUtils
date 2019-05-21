param(
    # Id of the session to remove
    [Parameter(Mandatory = $true)]
    [int[]]
    $SessionId
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version 2.0

$tsManager = New-Object Cassia.TerminalServicesManager
$server = $tsManager.GetLocalServer()

foreach ($currentSessionId in $SessionId)
{
    $session = $server.GetSession($currentSessionId)
    $session.Logoff()
}