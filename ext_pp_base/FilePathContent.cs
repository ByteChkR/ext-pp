using System.IO;

namespace ext_pp_base
{
    
    public class FilePathContent : IFileContent
    {
        private readonly string _filePath;
        private string _key;

        public FilePathContent(string filePath)
        {
            _key = _filePath = Path.GetFullPath(filePath);
        }
        public bool TryGetLines(out string[] lines)
        {

            
            lines = null;
            if (!File.Exists(_filePath))
            {
                return false;
            }
            lines = File.ReadAllLines(_filePath);

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