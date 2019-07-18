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
            public List<string> IncludedDirectories;

            public List<Tuple<string, string[], string, List<CommandMetaData>>> ManuallyIncluded_Prefixes;

            public List<Tuple<string, string[], string, List<CommandMetaData>>> IncludedFiles_Prefixes;


        }
        private readonly string _rootDir = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
        private string _configPath => Path.Combine(_rootDir, "plugin_manager.xml");
        private string _defaultPluginFolder => Path.Combine(_rootDir, "plugins");
        private PluginInformation info;
        private static XmlSerializer _serializer = new XmlSerializer(typeof(PluginInformation));

        public PluginManager()
        {
            if (!Directory.Exists(_defaultPluginFolder)) Directory.CreateDirectory(_defaultPluginFolder);
            this.Log(DebugLevel.LOGS, _configPath, Verbosity.LEVEL1);
            this.Log(DebugLevel.LOGS, _defaultPluginFolder, Verbosity.LEVEL1);
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
            info = (PluginInformation)_serializer.Deserialize(fs);
            fs.Close();

        }

        public void ListAllCachedData()
        {
            this.Log(DebugLevel.LOGS, "Listing all Cached Data:", Verbosity.LEVEL1);
            ListManualCachedPlugins();
            ListCachedFolders();
            ListCachedPlugins();
        }

        public void ListCachedHelpInfo()
        {
            this.Log(DebugLevel.LOGS, "Plugins [prefixes]:path", Verbosity.LEVEL1);
            for (int i = 0; i < info.IncludedFiles_Prefixes.Count; i++)
            {
                this.Log(DebugLevel.LOGS, info.IncludedFiles_Prefixes[i].Item2.Unpack("\n") + ":" + info.IncludedFiles_Prefixes[i].Item3, Verbosity.LEVEL1);
            }
        }

        public void ListCachedPlugins()
        {
            this.Log(DebugLevel.LOGS, "Plugins [prefixes]:path", Verbosity.LEVEL1);
            for (int i = 0; i < info.IncludedFiles_Prefixes.Count; i++)
            {
                this.Log(DebugLevel.LOGS, info.IncludedFiles_Prefixes[i].Item2.Unpack("\n") + ":" + info.IncludedFiles_Prefixes[i].Item3, Verbosity.LEVEL1);
            }
        }

        public void ListManualCachedPlugins()
        {
            this.Log(DebugLevel.LOGS, "Manually Included Plugins [prefixes]:path", Verbosity.LEVEL1);
            for (int i = 0; i < info.ManuallyIncluded_Prefixes.Count; i++)
            {
                this.Log(DebugLevel.LOGS, info.ManuallyIncluded_Prefixes[i].Item3 + "\n\t" + info.ManuallyIncluded_Prefixes[i].Item2.Unpack("\n\t"), Verbosity.LEVEL1);
            }
        }

        public void ListCachedFolders()
        {

            this.Log(DebugLevel.LOGS, "Directories:", Verbosity.LEVEL1);
            for (int i = 0; i < info.IncludedDirectories.Count; i++)
            {
                this.Log(DebugLevel.LOGS, info.IncludedDirectories[i], Verbosity.LEVEL1);
            }
        }

        public void AddFolder(string folder)
        {
            if (Directory.Exists(folder))
            {

                this.Log(DebugLevel.LOGS, "Adding Directory: " + folder, Verbosity.LEVEL1);
                info.IncludedDirectories.Add(Path.GetFullPath(folder));
            }
            else
            {
                this.Log(DebugLevel.ERRORS, "Folder does not exist: " + folder, Verbosity.LEVEL1);
            }

            Save();
        }

        private void AddFile(string file, bool save)
        {
            if (!File.Exists(file))
            {

                this.Log(DebugLevel.ERRORS, "File does not exist: " + file, Verbosity.LEVEL1);

            }
            else
            {
                file = Path.GetFullPath(file);
                if (info.ManuallyIncluded_Prefixes.Count(x => x.Item3 == file) != 0 || info.IncludedFiles_Prefixes.Count(x => x.Item3 == file) != 0) return;
                List<AbstractPlugin> plugins = FromFile(file);
                List<string> prefixes = new List<string>();

                plugins.ForEach(x => prefixes.AddRange(x.Prefix));
                this.Log(DebugLevel.LOGS, "Adding " + plugins.Count + " plugins from " + file,
                    Verbosity.LEVEL1);
                List<Tuple<string, string[], string, List<CommandMetaData>>> val = new List<Tuple<string, string[], string, List<CommandMetaData>>>();

                for (int i = 0; i < plugins.Count; i++)
                {
                    val.Add(new Tuple<string, string[], string, List<CommandMetaData>>(
                        plugins[i].GetType().Name, plugins[i].Prefix, file, plugins[i].Info.Select(x => x.Meta).ToList()));
                }

            }
            if (save) Save();
        }

        public bool ValueByName(string name, out Tuple<string, string[], string, List<CommandMetaData>> val)
        {
            for (int i = 0; i < info.IncludedFiles_Prefixes.Count; i++)
            {
                if (info.IncludedFiles_Prefixes[i].Item1 == name)
                {
                    val = info.IncludedFiles_Prefixes[i];
                    return true;
                }
            }

            for (int i = 0; i < info.ManuallyIncluded_Prefixes.Count; i++)
            {
                if (info.ManuallyIncluded_Prefixes[i].Item1 == name)
                {
                    val = info.ManuallyIncluded_Prefixes[i];
                    return true;
                }
            }

            val = null;
            return false;
        }

        public bool ValueByPrefix(string prefix, out Tuple<string, string[], string, List<CommandMetaData>> val)
        {
            for (int i = 0; i < info.IncludedFiles_Prefixes.Count; i++)
            {
                if (info.IncludedFiles_Prefixes[i].Item2.Contains(prefix))
                {
                    val = info.IncludedFiles_Prefixes[i];
                    return true;
                }
            }

            for (int i = 0; i < info.IncludedFiles_Prefixes.Count; i++)
            {
                if (info.IncludedFiles_Prefixes[i].Item2.Contains(prefix))
                {
                    val = info.IncludedFiles_Prefixes[i];
                    return true;
                }
            }

            val = null;
            return false;

        }

        public bool ValueByPathAndPrefix(string file, string prefix, out Tuple<string, string[], string, List<CommandMetaData>> val)
        {

            for (int i = 0; i < info.IncludedFiles_Prefixes.Count; i++)
            {
                if (info.IncludedFiles_Prefixes[i].Item3 == file && info.IncludedFiles_Prefixes[i].Item2.Contains(prefix))
                {
                    val = info.IncludedFiles_Prefixes[i];
                    return true;
                }
            }



            for (int i = 0; i < info.ManuallyIncluded_Prefixes.Count; i++)
            {
                if (info.ManuallyIncluded_Prefixes[i].Item3 == file)
                {
                    val = info.ManuallyIncluded_Prefixes[i];
                    return true;
                }
            }

            val = null;
            return false;
        }




        public bool DisplayHelp(string path, string[] names)
        {
            foreach (var name in names)
            {
                if (ValueByPathAndPrefix(path, name, out Tuple<string, string[], string, List<CommandMetaData>> val))
                {
                    this.Log(DebugLevel.LOGS, "Plugin Name: " + val.Item1, Verbosity.LEVEL1);
                    ListInfo(val.Item4.ToList());

                }
            }

            return true;
        }

        private void ListInfo(List<CommandMetaData> data)
        {
            foreach (var commandMetaData in data)
            {
                this.Log(DebugLevel.LOGS, "\n" + commandMetaData + "\n",
                    Verbosity.LEVEL1);
            }
        }

        public void AddFile(string file)
        {
            AddFile(file, true);

        }



        public void Refresh()
        {

            info.IncludedFiles_Prefixes.Clear();



            for (int i = info.IncludedDirectories.Count - 1; i >= 0; i--)
            {
                if (!Directory.Exists(info.IncludedDirectories[i]))
                {
                    this.Log(DebugLevel.ERRORS, "Folder does not exist: " + info.IncludedDirectories[i] + " Removing..",
                        Verbosity.LEVEL1);
                    info.IncludedDirectories.RemoveAt(i);

                }
                else
                {
                    this.Log(DebugLevel.LOGS, "Discovering Files in " + info.IncludedDirectories[i], Verbosity.LEVEL1);
                    string[] files = Directory.GetFiles(info.IncludedDirectories[i], "*.dll");
                    foreach (var file in files)
                    {
                        List<AbstractPlugin> plgin = FromFile(file);
                        List<string> prefixes = new List<string>();
                        plgin.ForEach(x => prefixes.AddRange(x.Prefix));
                        this.Log(DebugLevel.LOGS, "Adding " + plgin.Count + " plugins from " + file,
                            Verbosity.LEVEL1);
                        for (int j = 0; j < plgin.Count; j++)
                        {
                            info.IncludedFiles_Prefixes.Add(new Tuple<string, string[], string, List<CommandMetaData>>(
                                plgin[i].GetType().Name, plgin[i].Prefix, file, plgin[i].Info.Select(x => x.Meta).ToList()));
                        }
                    }
                }
            }

            List<string> manuallyIncluded = new List<string>(info.ManuallyIncluded_Prefixes.Select(x => x.Item3));
            info.ManuallyIncluded_Prefixes.Clear();



            foreach (var inc in manuallyIncluded)
            {
                AddFile(inc, false);
            }

            Save();
        }

        private List<AbstractPlugin> FromFile(string path)
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
                this.Log(DebugLevel.ERRORS, "Could not load file: " + path, Verbosity.LEVEL1);
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

            this.Log(DebugLevel.LOGS, "First start of Plugin Manager. Setting up...", Verbosity.LEVEL1);
            FileStream fs = new FileStream(_configPath, FileMode.Create);
            info = new PluginInformation()
            {
                IncludedFiles_Prefixes = new List<Tuple<string, string[], string, List<CommandMetaData>>>(),
                ManuallyIncluded_Prefixes = new List<Tuple<string, string[], string, List<CommandMetaData>>>(),
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


        public bool GetPath(string prefix, out string ret, out string name)
        {
            return GetPath(info.ManuallyIncluded_Prefixes, prefix, out ret, out name) ||
                   GetPath(info.IncludedFiles_Prefixes, prefix, out ret, out name);
        }


        private static bool GetPath(List<Tuple<string, string[], string, List<CommandMetaData>>> vals, string prefix, out string ret, out string name)
        {
            for (var index = 0; index < vals.Count; index++)
            {
                if (vals[index].Item2.Contains(prefix))
                {
                    ret = vals[index].Item3;
                    name = vals[index].Item1;
                    return true;
                }
            }

            name = "";
            ret = prefix;
            return false;
        }
    }
}