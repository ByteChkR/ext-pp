using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ext_pp_base.settings
{
    public class Settings
    {
        /// <summary>
        /// Settings with this prefix will be forwarded to any plugin in the chain
        /// </summary>
        public static string GlobalSettings = "glob";

        /// <summary>
        /// Dictionairy to store the settings for processing
        /// </summary>
        private readonly Dictionary<string, string[]> _settings;

        /// <summary>
        /// Default/Preset Constructor
        /// </summary>
        /// <param name="settings"></param>
        public Settings(Dictionary<string, string[]> settings = null)
        {
            _settings = settings ?? new Dictionary<string, string[]>();
        }

        /// <summary>
        /// Sets values in the settings
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Set(string key, string[] value)
        {
            if (_settings.ContainsKey(key))
            {
                _settings[key] = value;
            }
            else
            {
                _settings.Add(key, value);
            }
        }

        /// <summary>
        /// Sets a value in the settings
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Set(string key, string value)
        {
            Set(key, new[] { value });
        }

        /// <summary>
        /// returns the "first" value of the key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetFirst(string key)
        {
            return Get(key)[0];
        }

        /// <summary>
        /// Returns true if the settings contain this key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool HasKey(string key)
        {
            return _settings.ContainsKey(key);
        }


        /// <summary>
        /// returns the settings for the specified key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string[] Get(string key)
        {
            return _settings[key];
        }


        /// <summary>
        /// Returns the settings that have a prefix(e.g. are used in plugins)
        /// </summary>
        /// <param name="prefixes"></param>
        /// <param name="includeGlobalConfig"></param>
        /// <returns></returns>
        public Settings GetSettingsWithPrefix(string[] prefixes, bool includeGlobalConfig)
        {
            Dictionary<string, string[]> ret = new Dictionary<string, string[]>();
            Dictionary<string, string[]> tmp = new Dictionary<string, string[]>();
            for (int i = 0; i < prefixes.Length; i++)
            {
                tmp = GetSettingsWithPrefix(prefixes[i], includeGlobalConfig)._settings;
                foreach (var args in tmp)
                {
                    ret.Add(args.Key, args.Value);
                }
            }

            return new Settings(ret);
        }

        public Settings GetSettingsWithPrefix(string[] prefixes)
        {
            return GetSettingsWithPrefix(prefixes, false);
        }

        /// <summary>
            /// Returns a setting object that contains the settings with prefix.
            /// </summary>
            /// <param name="prefix"></param>
            /// <param name="argBegin"></param>
            /// <param name="includeShared"></param>
            /// <returns></returns>
            private Settings GetSettingsWithPrefix(string prefix, string argBegin, bool includeShared)
        {
            string prfx = argBegin + prefix + ":";
            bool isGlob;
            Dictionary<string, string[]> ret = new Dictionary<string, string[]>();
            foreach (var setting in _settings)
            {
                isGlob = false;
                if (setting.Key.StartsWith(prfx) ||
                    (isGlob = (includeShared && setting.Key.StartsWith(GlobalSettings))))
                {
                    ret.Add(setting.Key.Replace((isGlob ? GlobalSettings : prefix) + ":", ""), setting.Value);
                }
            }

            return new Settings(ret);
            //return new Settings(_settings.Where(x => x.Key.StartsWith(prfx) || (includeShared && x.Key.StartsWith(GlobalSettings)))
            //    .ToDictionary(x => x.Key.Replace(prefix + ":", ""), y => y.Value));
        }

        private Settings GetSettingsWithPrefix(string prefix, string argBegin)
        {
            return GetSettingsWithPrefix(prefix, argBegin, false);
        }

        /// <summary>
            /// Wrapper that returns the settings of the prefix.
            /// </summary>
            /// <param name="prefix"></param>
            /// <param name="includeShared"></param>
            /// <returns></returns>
            public Settings GetSettingsWithPrefix(string prefix, bool includeShared)
        {
            Settings s = GetSettingsWithPrefix(prefix, "--", includeShared);
            s = s.Merge(GetSettingsWithPrefix(prefix, "-", includeShared));
            return s;

        }

        public Settings GetSettingsWithPrefix(string prefix)
        {
            return GetSettingsWithPrefix(prefix, false);
        }

        private string[] FindCommandValue(CommandInfo c)
        {
            string key = "--" + c.Command;
            List<string> s = new List<string>();
            if (_settings.ContainsKey(key))
            {
                return _settings[key];
            }
            else if (c.ShortCut != "" && _settings.ContainsKey("-" + c.ShortCut)) return _settings["-" + c.ShortCut];
            {
                return null;
            }
        }

        /// <summary>
        /// Applies the settings with matching command infos.
        /// Using reflection and fieldinfos to set the values
        /// </summary>
        /// <param name="infos"></param>
        /// <param name="obj"></param>
        public void ApplySettings(List<CommandInfo> infos, object obj)
        {
            foreach (var commandInfo in infos)
            {
                if (commandInfo.Field.PropertyType.IsArray)
                {
                    ApplySettingArray(commandInfo, obj);
                }
                else
                {
                    ApplySettingFirst(commandInfo.Field.PropertyType, commandInfo, obj);
                }
            }
        }


        /// <summary>
        /// Applies the first index of the setting. and saves it in the fieldinfo in the command object.
        /// Automatically converts strings to almost all parsable objects
        /// </summary>
        /// <param name="t"></param>
        /// <param name="info"></param>
        /// <param name="obj"></param>
        public void ApplySettingFirst(Type t, CommandInfo info, object obj)
        {
            string[] cmdVal = FindCommandValue(info);
            if (cmdVal == null)
            {

                return;
            }
            object val = Utils.Parse(t, cmdVal.Length > 0 ? cmdVal.First() : null, info.DefaultIfNotSpecified);
            info.Field.SetValue(obj, val);
        }

        /// <summary>
        /// Applies the settings. and saves it in the fieldinfo in the command objects.
        /// Automatically converts strings and arrays to almost all parsable objects
        /// </summary>
        /// <param name="info"></param>
        /// <param name="obj"></param>
        public void ApplySettingArray(CommandInfo info, object obj)
        {
            string[] cmdVal = FindCommandValue(info);
            if (cmdVal == null) return;
            string[] val = Utils.ParseArray(info.Field.PropertyType.IsArray ?
                info.Field.PropertyType.GetElementType() :
                info.Field.PropertyType,
                cmdVal, info.DefaultIfNotSpecified)
                .OfType<string>().ToArray();
            info.Field.SetValue(obj, val);
        }


        /// <summary>
        /// Merges two settings objects.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Settings Merge(Settings other)
        {
            Settings s = new Settings(new Dictionary<string, string[]>(_settings));
            foreach (var otherSetting in other._settings)
            {
                s._settings.Add(otherSetting.Key, otherSetting.Value);
            }

            return s;
        }


    }
}