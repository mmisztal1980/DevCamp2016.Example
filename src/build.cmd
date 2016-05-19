@echo off

pushd %~dp0

SETLOCAL
SET CACHED_NUGET=%LocalAppData%\NuGet\NuGet.exe

IF EXIST %CACHED_NUGET% goto copynuget
echo Downloading latest version of NuGet.exe...
IF NOT EXIST %LocalAppData%\NuGet md %LocalAppData%\NuGet
@powershell -NoProfile -ExecutionPolicy unrestricted -Command "$ProgressPreference = 'SilentlyContinue'; Invoke-WebRequest 'https://www.nuget.org/nuget.exe' -OutFile '%CACHED_NUGET%'"

:copynuget
IF EXIST tools\.nuget\nuget.exe goto restore
md tools\.nuget
copy %CACHED_NUGET% tools\.nuget\nuget.exe > nul

:restore
tools\.nuget\NuGet.exe update -self
tools\.nuget\NuGet.exe install Cake -OutputDirectory tools -ExcludeVersion
tools\.nuget\NuGet.exe install Cake.XdtTransform -OutputDirectory tools -ExcludeVersion
tools\.nuget\NuGet.exe install Cake.PowerShell -OutputDirectory tools -ExcludeVersion
tools\.nuget\NuGet.exe install xunit.runner.console -OutputDirectory tools -ExcludeVersion
tools\.nuget\NuGet.exe install WinSCP -OutputDirectory tools -ExcludeVersion
tools\.nuget\NuGet.exe restore -SolutionDirectory .

:buildsolution
set encoding=utf-8
tools\Cake\Cake.exe build.cake %*

popd