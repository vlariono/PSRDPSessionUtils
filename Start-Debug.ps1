#region Header
$ErrorActionPreference = 'Stop'

$debugDataPath = Join-Path -Path $PSScriptRoot -ChildPath "Start-Debug.psd1"
$debugData = Import-PowerShellDataFile -Path $debugDataPath

$moduleName = 'PSRDPSessionUtils'
$moduleRootPath = Join-Path -Path $PSScriptRoot -ChildPath $moduleName
$moduleManifestPath = Join-Path -Path $moduleRootPath -ChildPath "$moduleName.psd1"

if(Get-Module -Name $moduleName)
{
    Remove-Module $moduleName
}

Import-Module $moduleManifestPath

$testUser = $debugData.Username
$testPass = $debugData.Password|ConvertTo-SecureString -AsPlainText -Force
$testCred = New-Object pscredential($testUser,$testPass)
#endregion

Get-RDPSession -ComputerName $debugData.ComputerName -Credential $testCred

