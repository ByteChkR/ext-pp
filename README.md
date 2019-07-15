# ext_pp
Small plugin based external text preprocessor that can introduce artificial static generics, different preprocessor keywords like #define #include #if and more to any kind of text file. Its main purpose is to keep me from copying my errors around when writing different opencl kernels.


You can find the documentation [HERE](https://bytechkr.github.io/ext-pp/)

## Status:
Master: [![Build Status](https://travis-ci.com/ByteChkR/ext-pp.svg?branch=master)](https://travis-ci.com/ByteChkR/ext-pp)  
Develop: [![Build Status](https://travis-ci.com/ByteChkR/ext-pp.svg?branch=develop)](https://travis-ci.com/ByteChkR/ext-pp)

## Usage:

### ext_pp
The core library of the Solution.
It contains all the nessecary components to run the text processor

### ext_pp_base
A Library containing interfaces to be able to develop plugins without recompiling the project

### ext_pp_cli
A console application that wraps around the core library and offers the full functionality from the command line.
To set a value for a plugin the syntax is:

	ext_pp_cli <command> value
	ext_pp_cli <pluginprefix>:<command> <value>

#### Valid Commands of the CLI itself:
	-i/-input
	-o/-output
	-defs => list of predefined variables
	-chain => sets the processing chain
	-l2f/-logToFile => writes the logs to the specified file
	-w2c/-writeToConsole => writes the finished text to the console(automatically sets the verbosity to silend if not specified otherwise.)
	-v/-verbosity => sets the amount of debug information(0 = none, 8 = ALL)

### ext_pp_plugins
A Library containing basic plugins

#### FakeGenericsPlugin
Prefix: "-gen"
Settings(cli_command): (default)

	+ GenericKeyword(g): "#type"
	+ Separator(s): " "
#### ConditionalPlugin
Prefix: "-con"
Settings(cli_command): (default)

	+ StartCondition(if): "#if"
	+ ElseIfCondition(elif): "#elseif"
	+ ElseCondition(else): "#elseif"
	+ EndCondition(eif): "#elseif"
	+ DefineKeyword(d): "#define"
	+ UndefineKeyword(u): "#undefine"
	+ OrOperator(o): "||"
	+ AndOperator(a): "&&"
	+ NotOperator(n): "!"
	+ Separator(s): " "
	+ EnableDefine(eD): true
	+ EnableUndefine(eU): true
#### IncludePlugin
Prefix: "-inc"

Settings(cli_command): (default)

	+ IncludeKeyword(i): "#include"
	+ Separator(s): " "
#### ErrorPlugin
Prefix: "-err"
Settings(cli_command): (default)

	+ ErrorKeyword(e): "#error"
	+ Separator(s): " "
#### WarningPlugin
Prefix: "-wrn"
Settings: (default)

	+ WarningKeyword(w): "#warning"
	+ Separator(s): " "
