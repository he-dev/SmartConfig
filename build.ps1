


msbuild /t:Rebuild /nologo /p:Configuration=Release /p:TargetFrameworkVersion=v4.0 /p:Platform="Any CPU" /p:OutDir="bin\Release\net40" `"$PSScriptRoot\SmartConfig.sln`"
msbuild /t:Rebuild /nologo /p:Configuration=Release /p:TargetFrameworkVersion=v4.5 /p:Platform="Any CPU" /p:OutDir="bin\Release\net45" `"$PSScriptRoot\SmartConfig.sln`"
msbuild /t:Rebuild /nologo /p:Configuration=Release /p:TargetFrameworkVersion=v4.5.1 /p:Platform="Any CPU" /p:OutDir="bin\Release\net451" `"$PSScriptRoot\SmartConfig.sln`"