$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

function script:Get-TempPs1File
{
    $tempFile = $null
    do
    {
        try
        {
            $tempFile = New-TemporaryFile -ErrorAction Stop
            $tempFile = Rename-Item -Path $tempFile.FullName -NewName "$($tempFile.BaseName).ps1" -Force -PassThru -ErrorAction Stop

        }
        catch [System.IO.IOException]
        {
            Write-Warning $_.Exception.Message
        }
    }
    while (!$tempFile)

    return $tempFile
}

function Connect-RDPSession
{
    <#
    .SYNOPSIS
        Connects to RDP session using mstsc
    .DESCRIPTION
        This cmdlet is just simple wrapper around cmdkey and mstsc. It uses cmdkey to save credential and mstsc to open RDP session
    .EXAMPLE
        PS C:\> Connect-RDPSession -ComputerName test.domain.tst -Credential $cred
    #>
    [CmdletBinding()]
    param(
        # Computer name
        [Parameter(Mandatory = $true)]
        [ValidateNotNullOrEmpty()]
        [string]
        $ComputerName,

        # Screen resolution in form of "wxh" (e.g. 1366x768), if parameter is not set full screen will be used
        [Parameter(Mandatory = $false)]
        [ValidatePattern('^\d+x\d+$')]
        [string]
        $Resolution,

        # Credential to connect with, if parameter is not set the credential will be asked with mstsc
        [Parameter(Mandatory = $false)]
        [ValidateNotNullOrEmpty()]
        [pscredential]
        $Credential,

        # If this parameter is set, server's rdp certificate will be added to trusted for mstsc
        [Parameter(Mandatory = $false)]
        [switch]
        $Force
    )

    if ($PSBoundParameters.ContainsKey('Force'))
    {
        $computerCertificate = Read-RDPCertificate -ComputerName $ComputerName
        $regPath = Join-Path -Path 'HKCU:\Software\Microsoft\Terminal Server Client\Servers' -ChildPath $ComputerName
        New-Item -Path $regPath -ItemType Container -Force | Out-Null
        $regValue = $computerCertificate.Thumbprint -replace '(\w\w)', '0x$1;' -split ';' | Where-Object { $_ } | ForEach-Object {
            [System.Convert]::ToChar([byte]$_)
        }
        New-ItemProperty -Path $regPath -Name 'CertHash' -PropertyType Binary -Value $regValue | Out-Null
    }

    if ($PSBoundParameters.ContainsKey('Credential'))
    {
        $userName = $Credential.UserName
        $userPassword = $Credential.GetNetworkCredential().Password

        cmdkey.exe /generic:$ComputerName /user:$userName /pass:$userPassword | Out-Null
        if ($LASTEXITCODE -gt 0)
        {
            Write-Error "Failed to save credential"
            return
        }
    }

    $cmdlineParams = , "/v:$ComputerName"
    if ($PSBoundParameters.ContainsKey('Resolution'))
    {
        if ($Resolution -match '^(?<Width>\d+)x(?<Height>\d+)$')
        {
            $cmdlineParams += "/w:$($Matches.Width)", "/h:$($Matches.Height)"
        }
        else
        {
            Write-Error "Incorrect resolution format $Resolution"
            return
        }
    }
    else
    {
        $cmdlineParams += '/f'
    }

    mstsc.exe $cmdlineParams
}

function Invoke-RDPSessionCommand
{
    <#
    .SYNOPSIS
        Invokes powershell script block in specified session
    .DESCRIPTION
        It's just wrapper around psexec. This cmdlet makes work with psexec more powershell native and simple
        This cmdlet checks exit code for errors, but does not return any output from remote command
    .EXAMPLE
        PS C:\> Invoke-RDPSessionCommand -ComputerName test.domain.tst -Credential $cred -SessionId 3 -ScriptBlock {
            param(
                [string]
                $TestP1,
                [string]
                $TestP2
            )
            echo $Test
        } -ArgumentList 'Hello','World'
    #>
    [CmdletBinding()]
    param(
        # Computer name
        [Parameter(Mandatory = $true)]
        [ValidateNotNullOrEmpty()]
        [string]
        $ComputerName,

        # Session ID to run command in
        [Parameter(Mandatory = $true)]
        [int]
        $SessionId,

        # Script block to run
        [Parameter(Mandatory = $true)]
        [ValidateNotNullOrEmpty()]
        [string]
        $ScriptBlock,

        # List of script arguments
        [Parameter(Mandatory = $false)]
        [ValidateNotNullOrEmpty()]
        [string[]]
        $ArgumentList,

        # Credential to connect with
        [Parameter(Mandatory = $true)]
        [ValidateNotNullOrEmpty()]
        [pscredential]
        $Credential
    )

    $binPath = Join-Path -Path $PSScriptRoot -ChildPath 'Bin'
    $libPath = Join-Path -Path $binPath -ChildPath 'Lib'
    $psexecPath = Join-Path -Path $libPath -ChildPath 'PsExec64.exe'
    Set-Alias -Name PSExec -Value $psexecPath

    $psSession = $null
    $localTempFile = $null
    $remoteTempFile = $null

    $psSessionOption = New-PSSessionOption -IncludePortInSPN
    $psSession = New-PSSession -ComputerName $ComputerName -Credential $Credential -SessionOption $psSessionOption
    if ($psSession)
    {
        try
        {
            $ErrorActionPreference = 'Stop'
            $localTempFile = script:Get-TempPs1File
            $remoteTempFile = Invoke-Command -Session $psSession -ScriptBlock ${function:script:Get-TempPs1File}

            Set-Content -Path $localTempFile -Value $ScriptBlock -Force
            Copy-Item -Path $localTempFile -Destination $remoteTempFile.FullName -Force -ToSession $psSession

            $userName = $Credential.UserName
            $userPswd = $Credential.GetNetworkCredential().Password
            PSExec -accepteula -nobanner "\\$ComputerName" -u $userName -p $userPswd -i $SessionId 'powershell' -NoProfile -NonInteractive -ExecutionPolicy 'Unrestricted' -File $remoteTempFile.FullName $ArgumentList| Out-Null
            if ($LASTEXITCODE -ne 0)
            {
                Write-Error "Execution of command on $ComputerName completed with error $LASTEXITCODE"
                return
            }
        }
        finally
        {
            if ($localTempFile)
            {
                Remove-Item -Path $localTempFile.FullName -Force
            }

            if ($remoteTempFile)
            {
                Invoke-Command -Session $psSession -ScriptBlock {
                    Remove-Item -Path $using:remoteTempFile.FullName -Force
                }
            }

            if ($psSession)
            {
                Remove-PSSession -Session $psSession -Confirm:$false
            }
        }
    }
}