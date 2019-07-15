namespace ext_pp_base
{
    public abstract class ADefinitions
    {
        public abstract void Set(string[] keys);
        public abstract void Unset(string[] keys);

        public abstract void Set(string key);
        public abstract void Unset(string key);
        public abstract bool Check(string key);
    }
}