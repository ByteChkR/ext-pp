# ext_pp
Small plugin based external text preprocessor that can introduce artificial static generics, different preprocessor keywords like #define #include #if and more to any kind of text file. Its main purpose is to keep me from copying my errors around when writing different opencl kernels.

You can find the documentation [HERE](https://bytechkr.github.io/ext-pp/)

## Status:
Master: [![Build Status](https://travis-ci.com/ByteChkR/ext-pp.svg?branch=master)](https://travis-ci.com/ByteChkR/ext-pp)  
Develop: [![Build Status](https://travis-ci.com/ByteChkR/ext-pp.svg?branch=develop)](https://travis-ci.com/ByteChkR/ext-pp)

## Quick Overview/Cheatsheet:

### ext_pp
The core library of the Solution.
It contains all the nessecary components to run the text processor

### ext_pp_base
A Library containing interfaces to be able to develop plugins without recompiling the project

## ext_pp_cli
A console application that wraps around the core library and offers the full functionality from the command line.
______________________________________________
### Console Commands:

	--input filepath,filepath filepath,filepath
		, separated = the same compilation
		[space] separated means gets queued after the compilation of the first one
	--output filepath filepath filepath
		, separated list of output files.
	--verbosity <int>
		Sets the debug output granularity
	--version
		displays the header
	--help <chainstr>
		lists the commands of the CLI or with supplied chain, it will display the help info of each plugin.
	--defines <vars>
		[space] separated list of predefines values
	--chain [filepath]
		[space] separated list of plugins
			filepath:pluginname:pluginname2 => loads a plugin by assembly name
			prefix => loads a plugin with prefix (has to be in /plugins folder)
			filepath:(collection) => loads a list(chain) of plugins from an IChainCollection with the specified name
			the plugins in the /plugin folder can be directly accessed by using the prefix instead of the lines above
	--log2file <file> <settings>
		Creates a log file with the settings
			<mask>:<timestamp>
			default: all:true
	--write2console [bool]
		Writes the result into the cout stream
		Sets the verbosity to silent if not specified otherwise
	--pm-refresh
		Refreshes the Plugin Manager.
	--pm-add <folder>
		Adds a folder with plugins to the Plugin Manager. All pluins in that folder can be referenced by their prefixes when specifies in --chain or --help
	--pm-list-dir
		Lists all Included dictionaries in Plugin Manager
	--pm-list-file
		Lists all Included and Cached Files in Plugin Manager
	--pm-list-manual-files
		Lists all Manually Included and Cached Files in Plugin Manager
	--pm-list-all
		Lists all Cached data.

### Plugins
______________________________________________
### Syntax Notes:

* a parameter that begins with @ is interpreted as file name
* 	[] mean required but user input
* 	<> means optional
* 	** is the default value
* 	| means different possible values
* 	no surrounding brackets means required characters or command
* 	all commands used in plugins have following layout: --[prefix]:[command] <args>

______________________________________________
#### Warning Plugin Information:

* Prefix: wrn, Warning
* Commands:

		set-warning [warning keyword] *#warning*
			Sets the keyword that is used to trigger warnings during compilation
		set-stage [OnLoad|OnFinishUp] *OnFinishUp*
			Sets the Stage Type of the Plugin to be Executed OnLoad or OnFinishUp
		set-order [Before|After] *After*
			Sets the Line Order to be Executed BEFORE the Fullscripts or AFTER the Fullscripts
		set-separator [separator keyword] * *
			Sets the separator that is used to separate different generic types
		

______________________________________________
#### Multi Line Plugin Information:

* Prefix: mlp, MultiLine
* Commands:

		set-mlkeyword [multipline keyword] *__*
			Sets the keyword that is used to detect when to lines should be merged. The line containing the keyword will be merges with the next line in the file
		set-stage [OnLoad|OnMain] *OnLoad*
			Sets the Stage Type of the Plugin to be Executed OnLoad or OnFinishUp

______________________________________________
#### Keyword Replacer Plugin Information:

* Prefix: kwr, KWReplacer
* Commands:

		set-surrkeyword [surrounding keyword] *$*
			Sets the keyword that is used to escape the variables. Usage: $VAR_NAME$
		no-defaultkeywords [bool] *false*
			Disables $TIME$, $DATE$ and $DATE_TIME$
		set-order [Before|After] *After*
			Sets the Line Order to be Executed BEFORE the Fullscripts or AFTER the Fullscripts
		set-stage [OnLoad|OnFinishUp] *OnFinishUp*
			Sets the Stage Type of the Plugin to be Executed OnLoad or OnFinishUp
		set-tformat [timeformatstring] *hh:mm:ss*
			Sets the time format string used when setting the default variables
		set-dformat [dateformatstring] *dd/MM/yyyy*
			Sets the date format string used when setting the default variables
		set-dtformat [datetimeformatstring] *dd/MM/yyyy hh:mm:ss*
			Sets the datetime format string used when setting the default variables

______________________________________________
#### Include Plugin Information:

* Prefix: inc, Include
* Commands:

		set-include [include keyword] *#include*
			Sets the keyword that is used to include other files into the build process.
		set-include-inline [include keyword] *#includeinl*
			Sets the keyword that is used to insert other files directly into the current file
		set-separator [separator keyword] * *
			Sets the separator that is used to separate the include statement from the filepath

______________________________________________
#### Fake Generics Plugin Information:

* Prefix: gen, FakeGen
* Commands:

		set-genkeyword [generic keyword] *#type*
			Sets the keyword that is used when writing pseudo generic code.
		set-stage [OnLoad|OnMain] *OnMain*
			Sets the Stage Type of the Plugin to be Executed OnLoad or OnFinishUp
		set-separator [separator keyword] * *
			Sets the separator that is used to separate different generic types

______________________________________________
#### Error Plugin Information:

* Prefix: err, Error
* Commands:

		set-error [error keyword] *#error*
			Sets the keyword that is used to trigger errors during compilation
		set-stage [OnLoad|OnFinishUp] *#OnFinishUp*
			Sets the Stage Type of the Plugin to be Executed OnLoad or OnFinishUp
		set-order [Before|After] *After*
			Sets the Line Order to be Executed BEFORE the Fullscripts or AFTER the Fullscripts
		set-separator [separator] * *
			Sets the separator that is used to separate the error keyword from the error text

______________________________________________
#### Conditional Plugin Information:

* Prefix:
	"con", "Conditional"
* Commands:

		set-define [define keyword] *#define*
			Sets the keyword that is used to define variables during the compilation.
		set-undefine [undefine keyword] *#undefine*
			Sets the keyword that is used to undefine previously defined variables during the compilation.
		set-if [if keyword] *#if*
			Sets the keyword that is used to start a new condition block.
		set-elseif [elseif keyword] *#elseif*
			Sets the keyword that is used to continue a previously started condition block with another condition block.
		set-else [else keyword] *#else*
			Sets the keyword that is used to start a new condition block that is taken when the previous blocks evaluated to false.
		set-endif [endif keyword] *#endif*
			Sets the keyword that is used to end a previously started condition block.
		set-not [not operator] *!*
			Sets the keyword that is used to negate an expression in if conditions.
		set-and [and operator] *&&*
			Sets the keyword for the logical AND operator
		set-or [or operator] *||*
			Sets the keyword for the logical OR operator
		enable-define [bool] *true*
			Enables/Disables the detection of define statements(defines can still be set via the defines object/the command line)
		enable-undefine [bool] *true*
			Enables/Disables the detection of undefine statements
		set-stage [OnLoad|OnMain] *OnMain*
			Sets the Stage Type of the Plugin to be Executed OnLoad or OnFinishUp
		set-separator [separator keyword] * *
			Sets the separator that is used to separate different generic types

______________________________________________
#### Blank Line Remover Plugin Information:

* Prefix: blk, BLRemover
* Commands:

		set-removekeyword [remove keyword] *###remove###*
			This will get inserted whenever a blank line is detected. This will be removed in the native cleanup of the PreProcessor
		set-order [Before|After] *After*
			Sets the Line Order to be Executed BEFORE the Fullscripts or AFTER the Fullscripts
		set-stage [OnLoad|OnFinishUp] *OnFinishUp*
			Sets the Stage Type of the Plugin to be Executed OnLoad or OnFinishUp

______________________________________________
Readme Generated by ext_pp