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
            public List<string> ManuallyIncluded_Values;
            public List<string[]> ManuallyIncluded_Keys;
            public List<string> IncludedFiles_Values;
            public List<string[]> IncludedFiles_Keys;

        }

        private readonly string _configPath = Path.GetFullPath("plugin_manager.xml");
        private readonly string _defaultPluginFolder = "plugins";
        private PluginInformation info;
        private static XmlSerializer _serializer = new XmlSerializer(typeof(PluginInformation));

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

        public void ListCachedPlugins()
        {
            this.Log(DebugLevel.LOGS, "Plugins [prefixes]:path", Verbosity.LEVEL1);
            for (int i = 0; i < info.IncludedFiles_Keys.Count; i++)
            {
                this.Log(DebugLevel.LOGS, info.IncludedFiles_Keys[i].Unpack("\n") + ":" + info.IncludedFiles_Values[i], Verbosity.LEVEL1);
            }
        }

        public void ListManualCachedPlugins()
        {
            this.Log(DebugLevel.LOGS, "Manually Included Plugins [prefixes]:path", Verbosity.LEVEL1);
            for (int i = 0; i < info.ManuallyIncluded_Keys.Count; i++)
            {
                this.Log(DebugLevel.LOGS, info.ManuallyIncluded_Values[i] + "\n\t" + info.ManuallyIncluded_Keys[i].Unpack("\n\t"), Verbosity.LEVEL1);
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
                if (info.ManuallyIncluded_Values.Contains(file) || info.IncludedFiles_Values.Contains(file)) return;
                List<AbstractPlugin> plugins = FromFile(file);
                List<string> prefixes = new List<string>();
                plugins.ForEach(x => prefixes.AddRange(x.Prefix));
                this.Log(DebugLevel.LOGS, "Adding " + plugins.Count + " plugins from " + file,
                    Verbosity.LEVEL1);
                info.ManuallyIncluded_Keys.Add(prefixes.ToArray());
                info.ManuallyIncluded_Values.Add(file);

            }
            if (save) Save();
        }

        public void AddFile(string file)
        {
            AddFile(file, true);

        }



        public void Refresh()
        {

            info.IncludedFiles_Keys.Clear();
            info.IncludedFiles_Values.Clear();


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
                        info.IncludedFiles_Keys.Add(prefixes.ToArray());
                        info.IncludedFiles_Values.Add(file);
                    }
                }
            }

            List<string> manuallyIncluded = new List<string>(info.ManuallyIncluded_Values);
            info.ManuallyIncluded_Values.Clear();
            info.ManuallyIncluded_Keys.Clear();

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
                IncludedFiles_Keys = new List<string[]>(),
                IncludedFiles_Values = new List<string>(),
                ManuallyIncluded_Values = new List<string>(),
                ManuallyIncluded_Keys = new List<string[]>(),
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


        public bool GetPath(string prefix, out string ret)
        {
            return GetPath(info.ManuallyIncluded_Keys, info.ManuallyIncluded_Values, prefix, out ret) ||
                   GetPath(info.IncludedFiles_Keys, info.IncludedFiles_Values, prefix, out ret);
        }


        private static bool GetPath(List<string[]> keys, List<string> values, string prefix, out string ret)
        {
            for (var index = 0; index < keys.Count; index++)
            {
                if (keys[index].Contains(prefix))
                {
                    ret = values[index];
                    return true;
                }
            }

            ret = prefix;
            return false;
        }
    }
}