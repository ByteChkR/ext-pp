using System.Collections.Generic;
using System.IO;
using System.Linq;
using ext_pp.settings;

namespace ext_pp
{
    public class SourceScript
    {

        public readonly string Filepath;
        public readonly string[] GenParam;
        private string[] _source = null;
        private readonly string _separator;
        public string[] Source
        {
            get
            {
                if (_source == null) Load();
                return _source;
            }
            set { _source = value; }
        }
        public string Key => Filepath + GenParamAppendix;
        


        private string GenParamAppendix
        {
            get
            {
                var gp = GenParam.Unpack(_separator);
                if (GenParam != null && GenParam.Length > 0) gp = "." + gp;
                return gp;
            }
        }


        public SourceScript(string separator, string path, string[] genParams)
        {
            GenParam = genParams;
            Filepath = path;
            _separator = separator;
        }


        private bool Load()
        {

            bool ret;
            if (!(ret = LoadSource()))
            {
            }

            return ret;
        }

        private bool LoadSource()
        {

            Source = new string[0];
            if (!File.Exists(Filepath)) return false;
            Source = File.ReadAllLines(Filepath);


            return true;
        }
        
    }
}

