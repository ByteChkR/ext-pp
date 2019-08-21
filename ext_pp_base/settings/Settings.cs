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
        public static string GlobalSettings { get; } = "glob";

        /// <summary>
        /// Dictionary to store the settings for processing
        /// </summary>
        private readonly Dictionary<string, string[]> _settings;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="settings">The settings in dictionary form</param>
        public Settings(Dictionary<string, string[]> settings)
        {
            _settings = settings ?? new Dictionary<string, string[]>();
        }

        /// <summary>
        /// Default Constructor
        /// </summary>
        public Settings() : this(null)
        {

        }

        /// <summary>
        /// Sets values in the settings
        /// </summary>
        /// <param name="key">The key to be set</param>
        /// <param name="value">the value that will be set</param>
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
        /// <param name="key">the key to be set</param>
        /// <param name="value">the value to be set</param>
        public void Set(string key, string value)
        {
            Set(key, new[] { value });
        }

        /// <summary>
        /// returns the "first" value of the key
        /// </summary>
        /// <param name="key">the key to be checked</param>
        /// <returns>the first entry of the value array</returns>
        public string GetFirst(string key)
        {
            return Get(key)[0];
        }

        /// <summary>
        /// Returns true if the settings contain this key
        /// </summary>
        /// <param name="key">the key to be checked</param>
        /// <returns>true if the key is contained.</returns>
        public bool HasKey(string key)
        {
            return _settings.ContainsKey(key);
        }


        /// <summary>
        /// returns the settings for the specified key
        /// </summary>
        /// <param name="key">the key to be used</param>
        /// <returns>the value array of the specified key</returns>
        public string[] Get(string key)
        {
            return _settings[key];
        }


        /// <summary>
        /// Returns the settings that have a prefix(e.g. are used in plugins)
        /// </summary>
        /// <param name="prefixes">The prefixes that will be included</param>
        /// <param name="includeGlobalConfig">flag to optionally also include settings with global prefix</param>
        /// <returns>The Settings object only containing settings with the specified prefixes(prefixes are removed)</returns>
        public Settings GetSettingsWithPrefix(string[] prefixes, bool includeGlobalConfig)
        {
            Dictionary<string, string[]> ret = new Dictionary<string, string[]>();

            for (int i = 0; i < prefixes.Length; i++)
            {
                Dictionary<string, string[]> tmp = GetSettingsWithPrefix(prefixes[i], includeGlobalConfig)._settings;
                foreach (var args in tmp)
                {
                    ret.Add(args.Key, args.Value);
                }
            }

            return new Settings(ret);
        }

        /// <summary>
        /// Returns the settings that have a prefix(e.g. are used in plugins)
        /// </summary>
        /// <param name="prefixes">The prefixes that will be included</param>
        /// <returns>The Settings object only containing settings with the specified prefixes(prefixes are removed)</returns>
        public Settings GetSettingsWithPrefix(string[] prefixes)
        {
            return GetSettingsWithPrefix(prefixes, false);
        }

        /// <summary>
        /// Returns a setting object that contains the settings with prefix.
        /// </summary>
        /// <param name="prefix">The prefix that will be included</param>
        /// <param name="argBegin">the char sequence used to detect prefixes</param>
        /// <param name="includeShared">a flag to optionally include settings with the global prefix</param>
        /// <returns>The Settings object only containing settings with the specified prefixes(prefixes are removed)</returns>
        private Settings GetSettingsWithPrefix(string prefix, string argBegin, bool includeShared)
        {
            string prfx = argBegin + prefix + ":";
            bool isGlob;
            Dictionary<string, string[]> ret = new Dictionary<string, string[]>();
            foreach (var setting in _settings)
            {
                isGlob = includeShared && setting.Key.StartsWith(GlobalSettings);
                if (setting.Key.StartsWith(prfx) || isGlob)
                {
                    ret.Add(setting.Key.Replace((isGlob ? GlobalSettings : prefix) + ":", ""), setting.Value);
                }
            }

            return new Settings(ret);
        }



        /// <summary>
        /// Wrapper that returns the settings of the prefix.
        /// </summary>
        /// <param name="prefix">The prefix that will be included</param>
        /// <param name="includeShared">a flag to optionally include settings with the global prefix</param>
        /// <returns>The Settings object only containing settings with the specified prefixes(prefixes are removed)</returns>
        public Settings GetSettingsWithPrefix(string prefix, bool includeShared)
        {
            Settings s = GetSettingsWithPrefix(prefix, "--", includeShared);
            s = s.Merge(GetSettingsWithPrefix(prefix, "-", includeShared));
            return s;

        }

        /// <summary>
        /// Wrapper that returns the settings of the prefix.
        /// </summary>
        /// <param name="prefix">The prefix that will be included</param>
        /// <returns></returns>
        public Settings GetSettingsWithPrefix(string prefix)
        {
            return GetSettingsWithPrefix(prefix, false);
        }

        /// <summary>
        /// A function that returns the settings for a specific command.
        /// </summary>
        /// <param name="c">The command info to be checked</param>
        /// <returns>the value array corresponding to the command info object</returns>
        private string[] FindCommandValue(CommandInfo c)
        {
            string key = "--" + c.Command;
            if (_settings.ContainsKey(key))
            {
                return _settings[key];
            }

            if (c.ShortCut != "" && _settings.ContainsKey("-" + c.ShortCut))
            {
                return _settings["-" + c.ShortCut];
            }

            return null;
        }

        /// <summary>
        /// Applies the settings with matching command infos.
        /// Using reflection and fieldinfos to set the values
        /// </summary>
        /// <param name="infos">The Command infos that the settings will be applied to</param>
        /// <param name="obj">The object that the reflection will set the value in</param>
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
        /// <param name="t">The type of the property to be set</param>
        /// <param name="info">the command info</param>
        /// <param name="obj">the class containing the property info of the commandinfo object</param>
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
        /// <param name="info">the command info</param>
        /// <param name="obj">the class containing the property info of the commandinfo object</param>
        public void ApplySettingArray(CommandInfo info, object obj)
        {
            string[] cmdVal = FindCommandValue(info);
            if (cmdVal == null)
            {
                return;
            }
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
        /// <param name="other">The other settings object that will be merged with this object</param>
        /// <returns>A merges settings object that will contain all the values of both settings objects.(Other settings will overwrite the settings of this object.)</returns>
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