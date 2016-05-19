<# File: Deploy.ps1
 # Author: @mmisztal1980
 # Desciption: Powershell script to upload files via FTP.
 # The script assumes that build.cmd has been run and that WinSCP NuGet package 
 # is present in tools/WinSCP. WinSCP is used to perform the FTP upload in a secure manner.
 #>

Param(
[Parameter(Mandatory=$true)][string] $hostName,
[Parameter(Mandatory=$true)][string] $userName,
[Parameter(Mandatory=$true)][string] $password,
[Parameter(Mandatory=$true)][string] $source)

Add-Type -Path ("{0}\..\tools\WinSCP\lib\WinSCPnet.dll" -f $PSScriptRoot);

$sessionOptions = New-Object WinSCP.SessionOptions -Property @{
    Protocol = [WinSCP.Protocol]::Ftp
    FtpSecure = [WinSCP.FtpSecure]::Explicit
    HostName = $hostName
    UserName = $userName
    Password = $password  
};

$session = New-Object WinSCP.Session -Property @{
    ExecutablePath = ("{0}\..\tools\WinSCP\content\winscp.exe" -f $PSScriptRoot);
};

$session.Open($sessionOptions)

$src = "{0}\..\{1}" -f $PSScriptRoot, $source;
Write-Host "Uploading from: "$src
[WinSCP.TransferOperationResult] $result = $session.PutFiles($src, "/site/wwwroot/")
$result.Check();

$session.Dispose()