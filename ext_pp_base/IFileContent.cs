namespace ext_pp_base
{
    public interface IFileContent
    {
        bool TryGetLines(out string[] lines);
        string GetKey();
        void SetKey(string key);
        string GetFilePath();


    }
}