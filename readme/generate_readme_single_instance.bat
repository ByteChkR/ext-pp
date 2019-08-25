@echo off
dotnet ../ext_pp_cli/bin/Release/netcoreapp2.1/ext_pp_cli.dll ^
-v 8 -pm-a ../ext_pp_plugins/bin/Release/netcoreapp2.1/ext_pp_plugins.dll -l2f process_output.log all:false __ ^
-v 8 -gen-r self ext_pp_cli/commands.md __ ^
-v 8 -gen-r ..\ext_pp_plugins\bin\Release\netcoreapp2.1\ext_pp_plugins.dll ..\PLUGIN_INFO.md __ ^
@generate_readme.args 
pause