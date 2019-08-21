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
		set-throw-on-warning/tow
			Enable this to throw on warnings.
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
