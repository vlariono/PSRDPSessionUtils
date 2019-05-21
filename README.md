# PSRDPSessionUtils
Collection of tools to work with RDP session

##Basic usage
1. Get credential
```Powershell
$cred = Get-Credential
```
2. Create new RDP session
```Powershell
New-RDPSession -ComputerName server1.domain.local -Credential $cred
```
3. Get RDP session info
```Powershell
Get-RDPSession -ComputerName  server1.domain.local -Credential $cred
```
4. Invoke command in RDP session
```Powershell
# Session id has been taken from output of Get-RDPSession
Invoke-RDPSessionCommand -ComputerName server1.domain.local -SessionId 2 -ScriptBlock {$true} -Credential $cred
```

5. Logoff rdp session
```Powershell
# Session id has been taken from output of Get-RDPSession
Remove-RDPSession -SessionId 2 -ComputerName server1.domain.local -Credential $cred
```

## Additional tools
1. Connect to rdp session
```Powershell
 Connect-RDPSession -ComputerName server1.domain.local -Credential $cred
```

2. Get RDP certificate
```Powershell
Read-RDPCertificate -ComputerName server1.domain.local
```

## Tools

This module is unification of different tools.
1. [FreeRDP](https://github.com/FreeRDP/FreeRDP): Session creation
2. [Cassia](https://github.com/danports/cassia): Session info and logoff
3. [PSExec](https://docs.microsoft.com/en-us/sysinternals/downloads/psexec): Invoke script block in specific session
4. Windows tools mstsc.exe and cmdkey.exe: Connection to RDP session