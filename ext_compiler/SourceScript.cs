namespace ext_compiler
{
    public class SourceScript
    {
        public string filepath;
        public string[] source;

        public SourceScript(string path, string[] source)
        {
            this.source = source;
            filepath = path;
        }

    }
}