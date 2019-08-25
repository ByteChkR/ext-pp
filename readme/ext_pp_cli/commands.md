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
		throw-on-warning/tow
			--throw-on-warning <true|false>
					Crashes the programm if any warnings are occuring.
		throw-on-error/toe
			--throw-on-error <true|false>
					Crashes the programm if any errors are occuring.
		generate-readme/gen-r
			--generate-readme <self|pathToPluginLibrary> <outputfile>
					Generates a readme file in markdown syntax.
