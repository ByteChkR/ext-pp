@echo off
echo Adding Default Plugins
dotnet ../ext_pp_cli/bin/Release/netcoreapp2.1/ext_pp_cli.dll -pm-a ../ext_pp_plugins/bin/Release/netcoreapp2.1/ext_pp_plugins.dll
echo Generating Readme from Library...
dotnet ../ext_pp_cli/bin/Release/netcoreapp2.1/ext_pp_cli.dll -readme ..\ext_pp_plugins\bin\Release\netcoreapp2.1\ext_pp_plugins.dll ..\PLUGIN_INFO.md
echo Compiling Readme...
dotnet ../ext_pp_cli/bin/Release/netcoreapp2.1/ext_pp_cli.dll @generate_readme.args
pause