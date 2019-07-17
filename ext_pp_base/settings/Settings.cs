using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ext_pp_base.settings
{
    public class Settings
    {
        public static string GlobalSettings = "glob";


        private readonly Dictionary<string, string[]> _settings;

        public Settings(Dictionary<string, string[]> settings = null)
        {
            _settings = settings ?? new Dictionary<string, string[]>();
        }

        public void Set(string key, string[] value)
        {
            if (_settings.ContainsKey(key)) _settings[key] = value;
            else _settings.Add(key, value);
        }

        public void Set(string key, string value)
        {
            Set(key, new[] { value });
        }

        public string GetFirst(string key)
        {
            return Get(key)[0];
        }

        public bool HasKey(string key)
        {
            return _settings.ContainsKey(key);
        }

        public string[] Get(string key)
        {
            return _settings[key];
        }

        public Settings GetSettingsWithPrefix(string[] prefixes, bool includeGlobalConfig = false)
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

        public Settings GetSettingsWithPrefix(string prefix, bool includeShared = false)
        {
            string prfx = "-" + prefix + ":";
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

        public void ApplySettings(List<CommandInfo> infos, object obj)
        {
            foreach (var commandInfo in infos)
            {
                if (commandInfo.Field.FieldType.IsArray) ApplySettingArray(commandInfo, obj);
                else ApplySettingFirst(commandInfo.Field.FieldType, commandInfo, obj);
            }
        }



        public void ApplySettingFirst(Type t, CommandInfo info, object obj)
        {
            string key = "-" + info.Command;
            if (!_settings.ContainsKey(key)) return;
            object val = Utils.Parse(t, _settings[key].Length > 0 ? _settings[key].First() : "");
            info.Field.SetValue(obj, val);
        }

        public void ApplySettingArray(CommandInfo info, object obj)
        {
            string key = "-" + info.Command;
            if (!_settings.ContainsKey(key) || _settings[key].Length == 0) return;
            string[] val = Utils.ParseArray(info.Field.FieldType.IsArray ?
                info.Field.FieldType.GetElementType() :
                info.Field.FieldType,
                _settings[key]).OfType<string>().ToArray();
            info.Field.SetValue(obj, val);
        }

    }
}