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

    public class PluginManager : ILoggable
    {
        [Serializable]
        public struct PluginInformation
        {
            public string[] Prefixes;
            public string Name;
            public string Path;
            public CommandMetaData[] Data;

            public PluginInformation(string[] prefixes, string name, string path, CommandMetaData[] data)
            {
                Path = path;
                Prefixes = prefixes;
                Name = name;
                Data = data;
            }

            public string GetDescription(bool shortDesc = true)
            {
                return Name + ": \n" + Path + "\nPrefixes:\n\t" + Prefixes.Unpack("\n\t") + (shortDesc ? "" : "\nCommand Info: \n\t" + Data.Select(x => x.ToString()).Unpack("\n\t"));
            }

            public override string ToString()
            {
                return GetDescription();
            }
        }

        [Serializable]
        public struct PluginManagerDatabase
        {
            public List<string> IncludedDirectories;
            public List<string> IncludedFiles;

            public List<PluginInformation> Cache;
        }
        private readonly string _rootDir = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
        private string _configPath => Path.Combine(_rootDir, "plugin_manager.xml");
        private string _defaultPluginFolder => Path.Combine(_rootDir, "plugins");
        private PluginManagerDatabase info;
        private static XmlSerializer _serializer = new XmlSerializer(typeof(PluginManagerDatabase));

        public PluginManager()
        {
            if (!Directory.Exists(_defaultPluginFolder)) Directory.CreateDirectory(_defaultPluginFolder);
            if (File.Exists(_configPath))
            {
                Initialize();
            }
            else
            {
                FirstStart();
            }
        }

        private void Initialize()
        {
            FileStream fs = new FileStream(_configPath, FileMode.Open);
            info = (PluginManagerDatabase)_serializer.Deserialize(fs);
            fs.Close();

        }

        public void ListAllCachedData()
        {
            this.Log(DebugLevel.LOGS, Verbosity.LEVEL1, "Listing all Cached Data:");
            ListCachedFolders();
            ListCachedPlugins(false);
        }

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

        public void ListCachedPlugins(bool shortDesc = false)
        {
            for (int i = 0; i < info.Cache.Count; i++)
            {
                this.Log(DebugLevel.LOGS, Verbosity.LEVEL1, info.Cache[i].GetDescription(shortDesc));
            }
        }

        public void ListCachedFolders()
        {

            this.Log(DebugLevel.LOGS, Verbosity.LEVEL1, "Directories:");
            for (int i = 0; i < info.IncludedDirectories.Count; i++)
            {
                this.Log(DebugLevel.LOGS, Verbosity.LEVEL1, info.IncludedDirectories[i]);
            }
        }

        public void ListManuallyCachedFiles()
        {

            this.Log(DebugLevel.LOGS, Verbosity.LEVEL1, "Directories:");
            for (int i = 0; i < info.IncludedFiles.Count; i++)
            {
                this.Log(DebugLevel.LOGS, Verbosity.LEVEL1, info.IncludedFiles[i]);
            }
        }

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

        private void AddFile(string file, bool save)
        {
            if (!File.Exists(file))
            {

                this.Log(DebugLevel.ERRORS, Verbosity.LEVEL1, "File does not exist: {0}", file);

            }
            else
            {
                file = Path.GetFullPath(file);
                if (info.Cache.Count(x => x.Path == file) != 0) return;

                info.IncludedFiles.Add(file);

                List<AbstractPlugin> plugins = FromFile(file);

                List<PluginInformation> val = new List<PluginInformation>();

                this.Log(DebugLevel.LOGS, Verbosity.LEVEL1, "Adding {0} plugins from {1}", plugins.Count, file);

                for (int i = 0; i < plugins.Count; i++)
                {
                    val.Add(new PluginInformation(plugins[i].Prefix, plugins[i].GetType().Name, file, plugins[i].Info.Select(x => x.Meta).ToArray()));
                }
                info.Cache.AddRange(val);

            }
            if (save) Save();
        }

        public bool GetPluginInfoByName(string name, out PluginInformation val)
        {
            for (int i = 0; i < info.Cache.Count; i++)
            {
                if (info.Cache[i].Name == name)
                {
                    val = info.Cache[i];
                    return true;
                }
            }


            val = new PluginInformation();
            return false;
        }

        public bool GetPluginInfoByPrefix(string prefix, out PluginInformation val)
        {
            for (int i = 0; i < info.Cache.Count; i++)
            {
                if (info.Cache[i].Prefixes.Contains(prefix))
                {
                    val = info.Cache[i];
                    return true;
                }
            }


            val = new PluginInformation();
            return false;

        }

        public bool GetPluginInfoByPathAndPrefix(string file, string prefix, out PluginInformation val)
        {

            for (int i = 0; i < info.Cache.Count; i++)
            {
                if (info.Cache[i].Path == file && info.Cache[i].Prefixes.Contains(prefix))
                {
                    val = info.Cache[i];
                    return true;
                }
            }



            val = new PluginInformation();
            return false;
        }

        private PluginInformation[] GetAllInLib(string pathToLib)
        {
            if (!File.Exists(pathToLib)) return new PluginInformation[0];
            List<PluginInformation> ret = new List<PluginInformation>();
            foreach (var info in info.Cache)
            {
                if (info.Path == pathToLib) ret.Add(info);
            }

            return ret.ToArray();
        }



        public bool DisplayHelp(string path, string[] names, bool shortDesc)
        {
            if (names == null)
            {
                PluginInformation[] inf = GetAllInLib(path);
                if (inf.Length == 0) return false;
                foreach (var name in inf)
                {

                    this.Log(DebugLevel.LOGS, Verbosity.LEVEL1, "\n{0}", name.GetDescription(shortDesc));

                }

                return true;

            }

            foreach (var name in names)
            {
                if (GetPluginInfoByPathAndPrefix(path, name, out PluginInformation val))
                {
                    this.Log(DebugLevel.LOGS, Verbosity.LEVEL1, "\n{0}", val.GetDescription(shortDesc));
                }
            }

            return true;
        }


        public void AddFile(string file)
        {
            AddFile(file, true);

        }



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
                    this.Log(DebugLevel.LOGS, Verbosity.LEVEL1, "Discovering Files in {0}" , info.IncludedDirectories[i]);
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
                this.Log(DebugLevel.ERRORS, Verbosity.LEVEL1, "Could not load file: {0}" , path);
                // ignored
            }

            return ret;
        }

        private void Save()
        {
            if (File.Exists(_configPath))
                File.Delete(_configPath);
            FileStream fs = new FileStream(_configPath, FileMode.Create);
            _serializer.Serialize(fs, info);
            fs.Close();
        }

        private void FirstStart()
        {

            this.Log(DebugLevel.LOGS, Verbosity.LEVEL1, "First start of Plugin Manager. Setting up...");
            FileStream fs = new FileStream(_configPath, FileMode.Create);
            info = new PluginManagerDatabase()
            {
                IncludedFiles = new List<string>(),
                Cache = new List<PluginInformation>(),
                IncludedDirectories = new List<string>()
                {
                    _defaultPluginFolder
                }
            };
            if (!Directory.Exists(_defaultPluginFolder)) Directory.CreateDirectory(_defaultPluginFolder);
            _serializer.Serialize(fs, info);
            fs.Close();
            Refresh();

        }



        public bool GetPathByName(string name, out string path)
        {
            if (GetPluginInfoByName(name, out var pli))
            {
                path = pli.Path;
                return true;
            }

            path = name;
            return false;
        }

        public bool GetPathByPrefix(string prefix, out string path)
        {
            if (GetPluginInfoByPrefix(prefix, out var pli))
            {
                path = pli.Path;
                return true;
            }

            path = prefix;
            return false;
        }



    }
}