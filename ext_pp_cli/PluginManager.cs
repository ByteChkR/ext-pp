using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using ext_pp_base;
using ext_pp_base.settings;

namespace ext_pp_cli
{
    /// <summary>
    /// Manages often used plugins and makes them available by prefix when added to the watched dirs or files.
    /// </summary>
    public class PluginManager : ILoggable
    {
        /// <summary>
        /// Basic information about the plugin
        /// </summary>
        [Serializable]
        public class PluginInformation
        {
            /// <summary>
            /// All the prefixes used by the plugin
            /// </summary>
            [XmlElement]
            public string[] Prefixes { get; set; }

            /// <summary>
            /// Assembly Name
            /// </summary>
            [XmlElement]
            public string Name { get; set; }

            /// <summary>
            /// Path of the library
            /// </summary>
            [XmlElement]
            public string Path { get; set; }

            /// <summary>
            /// The Cached information about each command.
            /// </summary>
            [XmlElement]
            public CommandMetaData[] Data { get; set; }

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="prefixes">the prefixes of the plugin</param>
            /// <param name="name">the name</param>
            /// <param name="path">the path</param>
            /// <param name="data">the meta data</param>
            public PluginInformation(string[] prefixes, string name, string path, CommandMetaData[] data)
            {
                Path = path;
                Prefixes = prefixes;
                Name = name;
                Data = data;
            }

            public PluginInformation()
            {

            }

            /// <summary>
            /// returns a description of the plugin
            /// </summary>
            /// <param name="shortDesc">flag to optionally only return a small description</param>
            /// <returns>the description of the Plugin</returns>
            public string GetDescription(bool shortDesc)
            {
                return Name + ": \n" + Path + "\nPrefixes:\n\t" + Prefixes.Unpack("\n\t") + (shortDesc ? "" : "\nCommand Info: \n\t" + Data.Select(x => x.ToString()).Unpack("\n\t"));
            }

            /// <summary>
            /// returns a description of the plugin
            /// </summary>
            /// <returns>the description of the Plugin</returns>
            public string GetDescription()
            {
                return GetDescription(true);
            }

            /// <summary>
            /// Overriden tostring method
            /// </summary>
            /// <returns>The content of GetDescription()</returns>
            public override string ToString()
            {
                return GetDescription();
            }
        }

        /// <summary>
        /// gets cached to disk.
        /// </summary>
        [Serializable]
        public class PluginManagerDatabase
        {
            /// <summary>
            /// The directories that will be automatically added when refreshed.
            /// </summary>
            public List<string> IncludedDirectories { get; set; } = new List<string>();

            /// <summary>
            /// the included files that were included manually.
            /// </summary>
            public List<string> IncludedFiles { get; set; } = new List<string>();

            /// <summary>
            /// The cache of the plugin information.
            /// </summary>

            public List<PluginInformation> Cache { get; set; } = new List<PluginInformation>();
            
        }
        /// <summary>
        /// Directory of the ext_pp_cli.dll library
        /// </summary>
        private readonly string _rootDir = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
        /// <summary>
        /// Config path for the cache
        /// </summary>
        private string _configPath => Path.Combine(_rootDir, "plugin_manager.xml");
        /// <summary>
        /// Default folder for plugins.
        /// </summary>
        private string _defaultPluginFolder => Path.Combine(_rootDir, "plugins");
        /// <summary>
        /// The Database of all plugins.
        /// </summary>
        private PluginManagerDatabase info;
        /// <summary>
        /// Serializer for loading/saving the cache to disk.
        /// </summary>
        private static XmlSerializer _serializer = new XmlSerializer(typeof(PluginManagerDatabase));


        /// <summary>
        /// Constructor
        /// </summary>
        public PluginManager()
        {
            if (!Directory.Exists(_defaultPluginFolder))
            {
                Directory.CreateDirectory(_defaultPluginFolder);
            }
            if (File.Exists(_configPath))
            {
                Initialize();
            }
            else
            {
                FirstStart();
            }
        }

        /// <summary>
        /// Initializes the PluginManager by loading the cache from file.
        /// </summary>
        private void Initialize()
        {
            FileStream fs = new FileStream(_configPath, FileMode.Open);
            info = (PluginManagerDatabase)_serializer.Deserialize(fs);
            fs.Close();

        }

        /// <summary>
        /// Lists all data that the cache contains.
        /// </summary>
        public void ListAllCachedData()
        {
            this.Log(DebugLevel.LOGS, Verbosity.LEVEL1, "Listing all Cached Data:");
            ListCachedFolders();
            ListCachedPlugins(false);
        }

        /// <summary>
        /// Lists the helpinfo of each plugin.(not used)
        /// </summary>
        public void ListCachedHelpInfo()
        {
            this.Log(DebugLevel.LOGS, Verbosity.LEVEL1, "Plugins [prefixes]:path");
            for (int i = 0; i < info.Cache.Count; i++)
            {
                this.Log(DebugLevel.LOGS, Verbosity.LEVEL1, "\n\n{0}", info.Cache[i].Name);
                for (int j = 0; j < info.Cache[i].Data.Length; j++)
                {
                    this.Log(DebugLevel.LOGS, Verbosity.LEVEL1, "\t{0}", info.Cache[i].Data[j].ToString());
                }
            }
        }

        /// <summary>
        /// Lists all cached plugins
        /// </summary>
        /// <param name="shortDesc">should skip the commands/help for commands</param>
        public void ListCachedPlugins(bool shortDesc)
        {
            for (int i = 0; i < info.Cache.Count; i++)
            {
                this.Log(DebugLevel.LOGS, Verbosity.LEVEL1, info.Cache[i].GetDescription(shortDesc));
            }
        }

        /// <summary>
        /// Returns all folders that are currently beeing watched.
        /// </summary>
        public void ListCachedFolders()
        {

            this.Log(DebugLevel.LOGS, Verbosity.LEVEL1, "Directories:");
            for (int i = 0; i < info.IncludedDirectories.Count; i++)
            {
                this.Log(DebugLevel.LOGS, Verbosity.LEVEL1, info.IncludedDirectories[i]);
            }
        }

        /// <summary>
        /// returns all the manually included files.
        /// </summary>
        public void ListManuallyCachedFiles()
        {

            this.Log(DebugLevel.LOGS, Verbosity.LEVEL1, "Manually Included Files:");
            for (int i = 0; i < info.IncludedFiles.Count; i++)
            {
                this.Log(DebugLevel.LOGS, Verbosity.LEVEL1, info.IncludedFiles[i]);
            }
        }

        /// <summary>
        /// Adds a folder to the PluginManager
        /// </summary>
        /// <param name="folder">The folder to be added</param>
        public void AddFolder(string folder)
        {
            if (Directory.Exists(folder))
            {

                this.Log(DebugLevel.LOGS, Verbosity.LEVEL1, "Adding Directory: {0}", folder);
                info.IncludedDirectories.Add(Path.GetFullPath(folder));
            }
            else
            {
                this.Log(DebugLevel.ERRORS, Verbosity.LEVEL1, "Folder does not exist: {0}", folder);
            }

            Save();
        }


        /// <summary>
        /// Adds a single file to the PluginManager
        /// </summary>
        /// <param name="file">the file to be added</param>
        /// <param name="save">flag to optionally save after adding</param>
        private void AddFile(string file, bool save)
        {
            if (!File.Exists(file))
            {

                this.Log(DebugLevel.ERRORS, Verbosity.LEVEL1, "File does not exist: {0}", file);

            }
            else
            {
                string fullpath = Path.GetFullPath(file);
                if (info.Cache.Count(x => x.Path == fullpath) != 0)
                {
                    return;
                }

                info.IncludedFiles.Add(fullpath);

                List<AbstractPlugin> plugins = FromFile(fullpath);

                List<PluginInformation> val = new List<PluginInformation>();

                this.Log(DebugLevel.LOGS, Verbosity.LEVEL1, "Adding {0} plugins from {1}", plugins.Count, fullpath);

                for (int i = 0; i < plugins.Count; i++)
                {
                    val.Add(new PluginInformation(plugins[i].Prefix, plugins[i].GetType().Name, fullpath, plugins[i].Info.Select(x => x.Meta).ToArray()));
                }
                info.Cache.AddRange(val);

            }

            if (save)
            {
                Save();
            }
        }

        /// <summary>
        /// Returns the plugin info by name. returns false if not containing
        /// </summary>
        /// <param name="pmd">The database that is cached</param>
        /// <param name="name">the name to be searched for</param>
        /// <param name="val">the out variable containing the plugin information(null if not found)</param>
        /// <returns>the success state of the operation</returns>
        public static bool TryGetPluginInfoByName(PluginManagerDatabase pmd, string name, out PluginInformation val)
        {
            for (int i = 0; i < pmd.Cache.Count; i++)
            {
                if (pmd.Cache[i].Name == name)
                {
                    val = pmd.Cache[i];
                    return true;
                }
            }


            val = new PluginInformation();
            return false;
        }

        /// <summary>
        /// Returns the plugin info by prefix. returns false if not containing
        /// </summary>
        /// <param name="pmd">The database that is cached</param>
        /// <param name="prefix">The prefix to be searched for</param>
        /// <param name="val">the out variable containing the plugin information(null if not found)</param>
        /// <returns>the success state of the operation</returns>
        public static bool TryGetPluginInfoByPrefix(PluginManagerDatabase pmd, string prefix, out PluginInformation val)
        {
            for (int i = 0; i < pmd.Cache.Count; i++)
            {
                if (pmd.Cache[i].Prefixes.Contains(prefix))
                {
                    val = pmd.Cache[i];
                    return true;
                }
            }


            val = new PluginInformation();
            return false;

        }

        /// <summary>
        /// Returns the plugin info by Path and Prefix. returns false if not containing
        /// </summary>
        /// <param name="pmd">The database that is cached</param>
        /// <param name="file">the file to be searched for</param>
        /// <param name="prefix">The prefix to be searched for</param>
        /// <param name="val">the out variable containing the plugin information(null if not found)</param>
        /// <returns>the success state of the operation</returns>
        public static bool TryGetPluginInfoByPathAndPrefix(PluginManagerDatabase pmd, string file, string prefix, out PluginInformation val)
        {

            for (int i = 0; i < pmd.Cache.Count; i++)
            {
                if (pmd.Cache[i].Path == file && pmd.Cache[i].Prefixes.Contains(prefix))
                {
                    val = pmd.Cache[i];
                    return true;
                }
            }



            val = new PluginInformation();
            return false;
        }

        /// <summary>
        /// Returns all cached information about this file.
        /// </summary>
        /// <param name="pathToLib">the file path to the compiled library</param>
        /// <returns>the plugin information of each plugin found</returns>
        private PluginInformation[] GetAllInLib(string pathToLib)
        {
            if (!File.Exists(pathToLib))
            {
                return new PluginInformation[0];
            }
            List<PluginInformation> ret = new List<PluginInformation>();
            foreach (var inf in info.Cache)
            {
                if (inf.Path == pathToLib)
                {
                    ret.Add(inf);
                }
            }

            return ret.ToArray();
        }


        /// <summary>
        /// Displays help for a specific file/plugin
        /// </summary>
        /// <param name="path">the path of the library containing the plugins</param>
        /// <param name="names">the names of the plugins to display help for</param>
        /// <param name="shortDesc">flag to optionally display a short description</param>
        /// <returns>the success state of the operation</returns>
        public bool DisplayHelp(string path, string[] names, bool shortDesc)
        {
            if (names == null)
            {
                PluginInformation[] inf = GetAllInLib(path);
                if (inf.Length == 0)
                {
                    return false;
                }
                foreach (var name in inf)
                {

                    this.Log(DebugLevel.LOGS, Verbosity.LEVEL1, "\n{0}", name.GetDescription(shortDesc));

                }

                return true;

            }

            foreach (var name in names)
            {
                if (TryGetPluginInfoByPathAndPrefix(info, path, name, out PluginInformation val))
                {
                    this.Log(DebugLevel.LOGS, Verbosity.LEVEL1, "\n{0}", val.GetDescription(shortDesc));
                }
            }

            return true;
        }


        /// <summary>
        /// Adds a file to the plugin manager
        /// </summary>
        /// <param name="file">The file to be added</param>
        public void AddFile(string file)
        {
            AddFile(file, true);

        }


        /// <summary>
        /// Refreshes the cache.
        /// </summary>
        public void Refresh()
        {

            info.Cache.Clear();



            for (int i = info.IncludedDirectories.Count - 1; i >= 0; i--)
            {
                if (!Directory.Exists(info.IncludedDirectories[i]))
                {
                    this.Log(DebugLevel.ERRORS, Verbosity.LEVEL1, "Folder does not exist: {0} Removing..", info.IncludedDirectories[i]);
                    info.IncludedDirectories.RemoveAt(i);

                }
                else
                {
                    this.Log(DebugLevel.LOGS, Verbosity.LEVEL1, "Discovering Files in {0}", info.IncludedDirectories[i]);
                    string[] files = Directory.GetFiles(info.IncludedDirectories[i], "*.dll");
                    foreach (var file in files)
                    {
                        List<AbstractPlugin> plugins = FromFile(file);
                        List<string> prefixes = new List<string>();
                        plugins.ForEach(x => prefixes.AddRange(x.Prefix));
                        this.Log(DebugLevel.LOGS, Verbosity.LEVEL1, "Adding {0} plugins from {1}", plugins.Count, file);
                        for (int j = 0; j < plugins.Count; j++)
                        {
                            info.Cache.Add(new PluginInformation(plugins[i].Prefix, plugins[i].GetType().Name, file, plugins[i].Info.Select(x => x.Meta).ToArray()));
                        }
                    }
                }
            }

            List<string> manuallyIncluded = new List<string>(info.IncludedFiles);
            info.IncludedFiles.Clear();



            foreach (var inc in manuallyIncluded)
            {
                AddFile(inc, false);
            }

            Save();
        }

        /// <summary>
        /// returns all plugins contained in the file.
        /// </summary>
        /// <param name="path">The path to a compiled assembly containing Absract Plugins</param>
        /// <returns>A list of all abstract plugins in the file</returns>
        public List<AbstractPlugin> FromFile(string path)
        {
            List<AbstractPlugin> ret = new List<AbstractPlugin>();
            try
            {
                Assembly asm = Assembly.LoadFile(Path.GetFullPath(path));
                Type[] types = asm.GetTypes();
                foreach (var type in types)
                {
                    if (type.IsSubclassOf(typeof(AbstractPlugin)))
                    {
                        ret.Add((AbstractPlugin)Activator.CreateInstance(type));
                    }
                }
            }
            catch (Exception)
            {
                this.Log(DebugLevel.ERRORS, Verbosity.LEVEL1, "Could not load file: {0}", path);
                // ignored
            }

            return ret;
        }


        /// <summary>
        /// Saves the cache to disk.
        /// </summary>
        private void Save()
        {
            if (File.Exists(_configPath))
            {
                File.Delete(_configPath);
            }
            FileStream fs = new FileStream(_configPath, FileMode.Create);
            _serializer.Serialize(fs, info);
            fs.Close();
        }

        /// <summary>
        /// Gets executed when its the first start.
        /// Will set up the cache and will perform a refresh.
        /// </summary>
        private void FirstStart()
        {

            this.Log(DebugLevel.LOGS, Verbosity.LEVEL1, "First start of Plugin Manager. Setting up...");
            FileStream fs = new FileStream(_configPath, FileMode.Create);
            info = new PluginManagerDatabase
            {
                IncludedFiles = new List<string>(),
                Cache = new List<PluginInformation>(),
                IncludedDirectories = new List<string>
                {
                    _defaultPluginFolder
                }
            };
            if (!Directory.Exists(_defaultPluginFolder))
            {
                Directory.CreateDirectory(_defaultPluginFolder);
            }
            _serializer.Serialize(fs, info);
            fs.Close();
            Refresh();

        }


        /// <summary>
        /// Returns the plugin path by name. returns false if not containing
        /// </summary>
        /// <param name="name">name to be searched for</param>
        /// <param name="path">the path of the assembly containing the specified plugin</param>
        /// <returns>the success state of the operation</returns>
        public bool TryGetPathByName(string name, out string path)
        {
            if (TryGetPluginInfoByName(info, name, out var pli))
            {
                path = pli.Path;
                return true;
            }

            path = name;
            return false;
        }

        /// <summary>
        /// Returns the plugin path by prefix. returns false if not containing
        /// </summary>
        /// <param name="prefix">prefix to be searched for</param>
        /// <param name="path">the path of the assembly containing the specified plugin</param>
        /// <returns>the success state of the operation</returns>
        public bool TryGetPathByPrefix(string prefix, out string path)
        {
            if (TryGetPluginInfoByPrefix(info, prefix, out var pli))
            {
                path = pli.Path;
                return true;
            }

            path = prefix;
            return false;
        }



    }
}