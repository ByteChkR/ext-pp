@echo off
echo Adding Default Plugins
dotnet ../ext_pp_cli/bin/Debug/netcoreapp2.1/ext_pp_cli.dll exit -pm-a ../ext_pp_plugins/bin/Debug/netcoreapp2.1/ext_pp_plugins.dll

echo Compiling Readme...
dotnet ../ext_pp_cli/bin/Debug/netcoreapp2.1/ext_pp_cli.dll exit @generate_document.args

pause