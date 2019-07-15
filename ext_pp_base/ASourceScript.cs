namespace ext_pp_base
{
    public abstract class ASourceScript
    {
        public abstract string GetFilePath();
        public abstract string GetKey();
        public abstract string[] GetSource();
        public abstract void SetSource(string[] source);
        public abstract bool Load();
        public abstract void AddValueToCache(string key, object value);
        public abstract bool HasValueOfType<T>(string key);
        public abstract T GetValueFromCache<T>(string key);
    }
}