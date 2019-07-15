using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ext_pp_base.settings
{
    public class Settings
    {
        public static string GlobalSettings = "-glob";
        public static string HelpText =
            "Parameter:\r\n-i|--input <path>\r\n-o|--output <path>\r\n Optional Parameter:	\t-rd|--resolveDefine [true|false]  \r\n\t-ru|--resolveUndefine [true|false]  \r\n\t-rc|--resolveConditions [true|false]  \r\n\t-ri|--resolveInclude [true|false]  \r\n\t-rg|--resolveGenerics [true|false]  \r\n\t-ee|--enableErrors [true|false]  \r\n\t-ew|--enableWarnings [true|false]  \r\n\t-def|--defines [DefineSymbols]  \r\n\t-v|--verbosity [0(Silent)-10(Maximum Debug Log)]\r\n\t-ss|--setSeparator [char]\r\n\t-2c|--writeToConsole\r\n\t-kw:d|--keyWord:d [defineStatement]\r\n\t-kw:u|--keyWord:u [unDefineStatement]\r\n\t-kw:if|--keyWord:if [ifStatement]\r\n\t-kw:elif|--keyWord:elif [elseIfStatement]\r\n\t-kw:else|--keyWord:else [elseStatement]\r\n\t-kw:eif|--keyWord:eif [endIfStatement]\r\n\t-kw:w|--keyWord:w [warningStatement]\r\n\t-kw:e|--keyWord:e [errorStatement]\r\n\t-kw:i|--keyWord:i [includeStatement]\r\n\t-kw:t|--keyWord:t [typeGenStatement]";

        private readonly Dictionary<string, string[]> _settings;

        public Settings(Dictionary<string, string[]> settings = null)
        {
            _settings = settings ?? new Dictionary<string, string[]>();
        }

        public void Set(string key, string[] value)
        {
            if (_settings.ContainsKey(key)) _settings[key] = value;
            else _settings.Add(key, value);
            Logger.Log(DebugLevel.LOGS, key + " changed to: " + value, Verbosity.LEVEL2);
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
                    ret.Add(setting.Key.Replace((isGlob ? GlobalSettings : "-" + prefix) + ":", ""), setting.Value);
                }
            }

            return new Settings(ret);
            //return new Settings(_settings.Where(x => x.Key.StartsWith(prfx) || (includeShared && x.Key.StartsWith(GlobalSettings)))
            //    .ToDictionary(x => x.Key.Replace(prefix + ":", ""), y => y.Value));
        }

        public void ApplySettingsFlatString(Dictionary<string, FieldInfo> _params, object obj)
        {
            foreach (var setting in _settings)
            {
                string set = setting.Key.Substring(1, setting.Key.Length - 1);
                if (_params.ContainsKey(set))
                {
                    if (setting.Value.Length == 0)
                        _params[set].SetValue(obj, "");
                    else
                        _params[set].SetValue(obj, setting.Value[0]);
                }
            }
        }
        public void ApplySettingsStringArray(Dictionary<string, FieldInfo> _params, object obj)
        {
            foreach (var setting in _settings)
            {
                if (_params.ContainsKey(setting.Key))
                {
                    _params[setting.Key].SetValue(obj, setting.Value);
                }
            }
        }




    }
}