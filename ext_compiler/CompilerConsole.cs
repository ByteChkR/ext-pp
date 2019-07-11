using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ext_compiler
{
    public static class CompilerConsole
    {
        static void Main(string[] args)
        {
            string input = "";
            string output = "";
            for (int i = 0; i < args.Length - 1; i++)
            {
                if (args[i] == "-i" || args[i] == "-input")
                {
                    string dirname = Path.GetDirectoryName(args[i + 1]);
                    if (dirname != "") Directory.SetCurrentDirectory(dirname);
                    input = args[i + 1];
                }
                else if (args[i] == "-o" || args[i] == "-output")
                {
                    output = args[i + 1];
                }
                else if (args[i] == "-tgen" || args[i] == "-typeGenericKeyword")
                {
                    ExtensionCompiler.TypeGenKeyword = args[i + 1];
                }
                else if (args[i] == "-inc" || args[i] == "-includeStatement")
                {
                    ExtensionCompiler.IncludeStatement = args[i + 1];
                }
            }

            if (input == "" || output == "")
            {
                Console.WriteLine("Invalid Command.\nUsage: -i inputpath -o outputpath");
            }

            
            string[] source = ExtensionCompiler.CompileFile(input);
            if (File.Exists(output))
                File.Delete(output);
            File.WriteAllLines(output, source);
#if DEBUG
            Console.ReadLine();
#endif
        }

        
    }
}
