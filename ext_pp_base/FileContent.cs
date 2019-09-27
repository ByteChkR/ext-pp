using System.IO;

namespace ext_pp_base
{
    public interface IFileContent
    {
        bool TryGetLines(out string[] lines);
        string GetKey();
        void SetKey(string key);
        string GetFilePath();

    }


    public class FilePathContent : IFileContent
    {
        private readonly string _filePath;
        private string _key;

        public FilePathContent(string filePath)
        {
            _key = _filePath = filePath;
        }
        public bool TryGetLines(out string[] lines)
        {

            string f = Path.GetFullPath(_filePath);
            Directory.SetCurrentDirectory(Path.GetDirectoryName(f));
            lines = null;
            if (!File.Exists(f))
            {
                return false;
            }
            lines = File.ReadAllLines(f);

            return true;
        }

        public string GetKey()
        {
            return _key;
        }

        public void SetKey(string key)
        {
            _key = key;
        }

        public string GetFilePath()
        {
            return Path.GetFullPath(_filePath);
        }
    }
}