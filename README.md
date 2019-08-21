# ext_pp
Small plugin based external text preprocessor that can introduce artificial static generics, different preprocessor keywords like #define #include #if and more to any kind of text file. Its main purpose is to keep me from copying my errors around when writing different opencl kernels.

You can find the documentation [HERE](https://bytechkr.github.io/ext-pp/)

## Status:
Code Quality: [![Codacy Badge](https://api.codacy.com/project/badge/Grade/94c3df94ad954b8b90dbb5a3b0832a33)](https://www.codacy.com/app/ByteChkR/ext-pp?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=ByteChkR/ext-pp&amp;utm_campaign=Badge_Grade)  
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

		input/i
			--input filepath,filepath filepath,filepath
				, separated = the same compilation
				[space] separated means gets queued after the compilation of the first one
		output/o
			--output filepath filepath filepath
				[space] separated list of output files.
		defines/d
			--defines <vars>
				, [space] separated list of predefines values
		chain/c
			--chain [filepath]
				, separated list of plugins
					[filepath]:pluginname => loads a plugin by assembly name
					[filepath]:prefix => loads a plugin with prefix
					[filepath]:(collection) => loads a list(chain) of plugins from an IChainCollection with the specified name
					the plugins in the /plugin folder can be directly accessed by using the prefix instead of the lines above
		log-to-file/l2f
			--log-to-file <file> <settings>
				Creates a log file with the settings
					<mask>:<timestamp>
					default: all:true
		write-to-console/w2c
			--write2console [bool]
				Writes the result into the cout stream
				Sets the verbosity to silent if not specified otherwise
		verbosity/v
			--verbosity <int>
				Sets the debug output granularity
		version/vv
			--version
				displays the current version
		no-chain-collection/nc
			The CLI will not search for a ChainCollection in the specified assembly
		help/h
					--help <chainstr>
					lists the commands of the CLI or with supplied chain, it will display the help info of each plugin.
		help-all/hh
					--help-all <chainstr>
					lists the commands of the CLI or with supplied chain, it will display the help info of each plugin.
		pm-refresh/pm-r
			--pm-refresh
					Refreshes the Plugin Manager.
		pm-add/pm-a
			--pm-add <folder>
					Adds a folder with plugins to the Plugin Manager. All pluins in that folder can be referenced by their prefixes when specifies in --chain or --help
		pm-list-dir/pm-ld
			--pm-list-dir
					Lists all Included dictionaries in Plugin Manager
		pm-list-file/pm-lf
			--pm-list-file
					Lists all Included and Cached Files in Plugin Manager
		pm-list-manual-files/pm-lmf
			--pm-list-manual-files
					Lists all Manually Included and Cached Files in Plugin Manager
		pm-list-all/pm-la
			--pm-list-all
					Lists all Cached data.
		generate-readme/gen-r
			--generate-readme <sourceLib> <targetFile>
					Generates a Markdown Readme of the Plugins in the source lib.

______________________________________________
#### BlankLineRemover Information:

* Prefix: blr, BLRemover
* Commands:

		set-removekeyword/k
			This will get inserted whenever a blank line is detected. This will be removed in the native cleanup of the PreProcessor
		set-order/o
			Sets the Line Order to be Executed BEFORE the Fullscripts or AFTER the Fullscripts
		set-stage/ss
			Sets the Stage Type of the Plugin to be Executed OnLoad or OnFinishUp
______________________________________________
#### ChangeCharCase Information:

* Prefix: ccc, ChangeCharCase
* Commands:

		set-order/o
			Sets the Line Order to be Executed BEFORE the Fullscripts or AFTER the Fullscripts
		set-stage/ss
			Sets the Stage Type of the Plugin to be Executed OnLoad or OnFinishUp
		set-case/sc
			Sets the Case that will transform the text. Options: tolower(default)/toupper
______________________________________________
#### ConditionalPlugin Information:

* Prefix: con, Conditional
* Commands:

		set-define/d
			Sets the keyword that is used to define variables during the compilation.
		set-undefine/u
			Sets the keyword that is used to undefine previously defined variables during the compilation.
		set-if/if
			Sets the keyword that is used to start a new condition block.
		set-elseif/elif
			Sets the keyword that is used to continue a previously started condition block with another condition block.
		set-else/else
			Sets the keyword that is used to start a new condition block that is taken when the previous blocks evaluated to false.
		set-endif/eif
			Sets the keyword that is used to end a previously started condition block.
		set-not/n
			Sets the keyword that is used to negate an expression in if conditions.
		set-and/a
			Sets the keyword for the logical AND operator
		set-or/o
			Sets the keyword for the logical OR operator
		enable-define/eD
			Enables/Disables the detection of define statements(defines can still be set via the defines object/the command line)
		enable-undefine/eU
			Enables/Disables the detection of undefine statements
		set-stage/ss
			Sets the Stage Type of the Plugin to be Executed OnLoad or OnFinishUp
		set-separator/s
			Sets the separator that is used to separate different generic types
______________________________________________
#### ExceptionPlugin Information:

* Prefix: ex, ExceptionPlugin
* Commands:

		set-error/e
			Sets the keyword that is used to trigger errors during compilation
		set-warning/w
			sets the keyword that is used to trigger warnings during compilation
		set-separator/s
			Sets the separator that is used to separate different generic types
		set-order/o
			Sets the Line Order to be Executed BEFORE the Fullscripts or AFTER the Fullscripts
		set-stage/ss
			Sets the Stage Type of the Plugin to be Executed OnLoad or OnFinishUp
______________________________________________
#### FakeGenericsPlugin Information:

* Prefix: gen, FakeGen
* Commands:

		set-genkeyword/g
			Sets the keyword that is used when writing pseudo generic code.
		set-separator/s
			Sets the separator that is used to separate different generic types
		set-stage/ss
			Sets the Stage Type of the Plugin to be Executed OnLoad or OnFinishUp
______________________________________________
#### IncludePlugin Information:

* Prefix: inc, Include
* Commands:

		set-include/i
			Sets the keyword that is used to include other files into the build process.
		set-include-inline/ii
			Sets the keyword that is used to insert other files directly into the current file
		set-separator/s
			Sets the separator that is used to separate the include statement from the filepath
______________________________________________
#### KeyWordReplacer Information:

* Prefix: kwr, KWReplacer
* Commands:

		set-order/o
			Sets the Line Order to be Executed BEFORE the Fullscripts or AFTER the Fullscripts
		set-stage/ss
			Sets the Stage Type of the Plugin to be Executed OnLoad or OnFinishUp
		no-defaultkeywords/nod
			Disables $TIME$, $DATE$ and $DATE_TIME$
		set-dtformat/dtf
			Sets the datetime format string used when setting the default variables
		set-tformat/tf
			Sets the time format string used when setting the default variables
		set-dformat/df
			Sets the date format string used when setting the default variables
		set-surrkeyword/sc
			Sets the Surrounding char that escapes the variable names
		set-kwdata/kwd
			Sets the Keywords that need to be replaced with values. <keyword>:<value>
______________________________________________
#### MultiLinePlugin Information:

* Prefix: mlp, MultiLine
* Commands:

		set-stage/ss
			Sets the Stage Type of the Plugin to be Executed OnLoad or OnFinishUp
		set-mlkeyword/mlk
			Sets the keyword that is used to detect when to lines should be merged. The line containing the keyword will be merges with the next line in the file
______________________________________________
#### TextEncoderPlugin Information:

* Prefix: tenc, TextEncoderPlugin
* Commands:

		set-stage/ss
			Sets the Stage Type of the Plugin to be Executed OnLoad or OnMain
		set-start-encode-keyword/ssek
			Sets the keyword that is used to open a Encode block
		set-end-encode-keyword/seek
			Sets the keyword that is used to end a Encode block
		set-start-decode-keyword/ssdk
			Sets the keyword that is used to open a Decode block
		set-end-decode-keyword/sedk
			Sets the keyword that is used to end a Decode block
______________________________________________
Readme Generated by ext_pp (21/08/2019 07:27:32)