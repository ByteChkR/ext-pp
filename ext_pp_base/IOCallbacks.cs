using System.IO;

namespace ext_pp_base
{
    public class IOCallbacks
    {
        public static IOCallbacks Callback;

        static IOCallbacks()
        {
            Callback = new IOCallbacks();
        }

        public virtual bool FileExists(string file)
        {
            return File.Exists(file);
        }

        public virtual string[] ReadAllLines(string file)
        {
            return File.ReadAllLines(file);
        }

        public virtual string[] GetFiles(string path, string searchPattern = "*.*")
        {
            return Directory.GetFiles(path, searchPattern, SearchOption.AllDirectories);
        }
    }
}