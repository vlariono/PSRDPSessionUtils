param(
	# Base64 representation of cassia assembly
    [Parameter(Mandatory = $true)]
    [ValidateNotNullOrEmpty()]
    [string]
    $CassiaAssemblyBase64
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version 2.0

$cassiaAssemblyByte = [System.Convert]::FromBase64String($cassiaAssemblyBase64)
[System.Reflection.Assembly]::Load($cassiaAssemblyByte)|Out-Null