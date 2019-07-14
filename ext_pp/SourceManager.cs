using System.Collections.Generic;
using System.Linq;

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
            int idx = _sources.IndexOfFile(script.Key);
            var a = _sources[idx];
            var ab = _doneState[idx];
            _doneState.RemoveAt(idx);
            _doneState.Add(ab);
            _sources.RemoveAt(idx);
            _sources.AddFile(a, true);
        }


        //public bool IsOnTodo(SourceScript script)
        //{
        //    return IsIncluded(script) && _doneState[_sources.IndexOfFile(script.Key)];
        //}
        public bool IsIncluded(SourceScript script)
        {
            return _sources.Any(x => x.Key == script.Key);
        }

        public void AddToTodo(SourceScript script)
        {
            if (!IsIncluded(script))
            {
                _sources.AddFile(script, false);
                _doneState.Add(false);
            }
        }

        public void SetDone(SourceScript script)
        {
            if (IsIncluded(script)) _doneState[_sources.IndexOfFile(script.Key)] = true;
        }

        public List<SourceScript> GetList()
        {
            return _sources;
        }
    }
}