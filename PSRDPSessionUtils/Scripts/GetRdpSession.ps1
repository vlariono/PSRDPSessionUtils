$ErrorActionPreference = 'Stop'
Set-StrictMode -Version 2.0

$tsManager = New-Object Cassia.TerminalServicesManager
$server = $tsManager.GetLocalServer()
$server.GetSessions()