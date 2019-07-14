using System.Collections.Generic;
using System.Linq;
using ext_pp.settings;

namespace ext_pp
{
    public class SourceManager
    {
        private List<SourceScript> _sources = new List<SourceScript>();
        private List<bool> _doneState = new List<bool>();

        public SourceScript NextItem
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

        public void FixOrder(SourceScript script)
        {
            Logger.Log(DebugLevel.LOGS, "Fixing Build Order of file: " +script.Key, Verbosity.LEVEL2);
            int idx = IndexOfFile(script.Key);
            var a = _sources[idx];
            var ab = _doneState[idx];
            _doneState.RemoveAt(idx);
            _doneState.Add(ab);
            _sources.RemoveAt(idx);
            AddFile(a, true);
        }


        
        public bool IsIncluded(SourceScript script)
        {
            return _sources.Any(x => x.Key == script.Key);
        }

        public void AddToTodo(SourceScript script)
        {
            if (!IsIncluded(script))
            {
                Logger.Log(DebugLevel.LOGS, "Adding Script to Todo List: " + script.Key, Verbosity.LEVEL2);
                AddFile(script, false);
                _doneState.Add(false);
            }
        }

        public void SetDone(SourceScript script)
        {
            if (IsIncluded(script))
            {
                _doneState[IndexOfFile(script.Key)] = true;

                Logger.Log(DebugLevel.LOGS, "Finished Script: " + script.Key, Verbosity.LEVEL2);
            }
        }

        public List<SourceScript> GetList()
        {
            return _sources;
        }

        public  void AddFile(SourceScript script, bool checkForExistingKey)
        {
            if (checkForExistingKey && ContainsFile(script.Key)) return;
            _sources.Add(script);

        }

        public bool ContainsFile(string key)
        {
            return IndexOfFile(key) != -1;
        }

        public int IndexOfFile(string key)
        {
            for (var i = 0; i < _sources.Count; i++)
            {
                if (_sources[i].Key == key) return i;
            }

            return -1;
        }
    }
}