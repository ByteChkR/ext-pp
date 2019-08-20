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