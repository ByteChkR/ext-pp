using System.Collections.Generic;
using System.IO;
using System.Linq;
using ext_pp_base;
using ext_pp_base.settings;

namespace ext_pp
{
    public class SourceManager : ASourceManager
    {

        private readonly List<ASourceScript> _sources = new List<ASourceScript>();
        private readonly List<bool> _doneState = new List<bool>();

        public SourceManager()
        {
            if (!SetScheme)
                KeyComputingScheme = ComputeFileNameAndKey_Default;
        }

        private static bool ComputeFileNameAndKey_Default(string[] vars, out string filePath, out string key, out Dictionary<string, object> pluginCache)
        {
            pluginCache = new Dictionary<string, object>();
            filePath = key = "";
            if (vars.Length == 0) return false;
            key =
                filePath = Path.GetFullPath(vars[0]);
            return true;
        }

        public ASourceScript NextItem
        {
            get
            {
                for (int i = 0; i < _doneState.Count; i++)
                {
                    if (!_doneState[i])
                    {
                        return _sources[i];
                    }
                }

                return null;
            }
        }

        public override void FixOrder(ASourceScript script)
        {
            Logger.Log(DebugLevel.LOGS, "Fixing Build Order of file: " + script.GetKey(), Verbosity.LEVEL2);
            int idx = IndexOfFile(script.GetKey());
            var a = _sources[idx];
            var ab = _doneState[idx];
            _doneState.RemoveAt(idx);
            _doneState.Add(ab);
            _sources.RemoveAt(idx);
            AddFile(a, true);
        }



        public override bool IsIncluded(ASourceScript script)
        {
            return _sources.Any(x => x.GetKey() == script.GetKey());
        }

        public override void AddToTodo(ASourceScript script)
        {
            if (!IsIncluded(script))
            {
                Logger.Log(DebugLevel.LOGS, "Adding Script to Todo List: " + script.GetKey(), Verbosity.LEVEL2);
                AddFile(script, false);
                _doneState.Add(false);
            }
        }

        public void SetDone(ASourceScript script)
        {
            if (IsIncluded(script))
            {
                _doneState[IndexOfFile(script.GetKey())] = true;

                Logger.Log(DebugLevel.LOGS, "Finished Script: " + script.GetKey(), Verbosity.LEVEL2);
            }
        }

        public List<ASourceScript> GetList()
        {
            return _sources;
        }

        private void AddFile(ASourceScript script, bool checkForExistingKey)
        {
            if (checkForExistingKey && ContainsFile(script.GetKey())) return;
            _sources.Add(script);

        }

        private bool ContainsFile(string key)
        {
            return IndexOfFile(key) != -1;
        }

        public override int IndexOfFile(string key)
        {
            for (var i = 0; i < _sources.Count; i++)
            {
                if (_sources[i].GetKey() == key) return i;
            }

            return -1;
        }

        public override ASourceScript CreateScript(string separator, string file, string key, Dictionary<string, object> pluginCache)
        {
            return new SourceScript(separator, file, key, pluginCache);
        }
    }
}