using System.Collections.Generic;
using System.IO;
using ext_pp_base;
using ext_pp_base.settings;
using NUnit.Framework;

namespace ext_pp.tests
{
    public static class TestHelper
    {


        private static string _resourceFolder { get; } = Path.GetFullPath("../../../res/");
        public static string ResourceFolder { get; private set; } = "";
        
        public static void SetupPath()
        {
            if (ResourceFolder == "")
            {
                ResourceFolder = _resourceFolder;
            }
        }

        private static PreProcessor setUp(List<AbstractPlugin> chain)
        {
            PreProcessor pp = new PreProcessor();
            pp.SetFileProcessingChain(chain);
            return pp;
        }

        public static ISourceScript[] SetUpAndProcess(List<AbstractPlugin> chain, Settings settings, IDefinitions definitions, params string[] fileNames)
        {

            PreProcessor pp = setUp(chain);
            return pp.Process(fileNames, settings, definitions);
        }


        public static ISourceScript[] SetUpAndProcess(List<AbstractPlugin> chain, params string[] fileNames)
        {
            return SetUpAndProcess(chain, new Settings(), new Definitions(), fileNames);
        }

        public static ISourceScript[] SetUpAndProcess(List<AbstractPlugin> chain, IDefinitions definitions,
            params string[] fileNames)
        {

            return SetUpAndProcess(chain, new Settings(), definitions, fileNames);
        }

        public static ISourceScript[] SetUpAndProcess(List<AbstractPlugin> chain, Settings settings, params string[] fileNames)
        {
            return SetUpAndProcess(chain, settings, new Definitions(), fileNames);
        }

        public static string[] SetUpAndCompile(List<AbstractPlugin> chain, Settings settings, IDefinitions definitions, params string[] fileNames)
        {
            PreProcessor pp = setUp(chain);
            return pp.Compile(fileNames, settings, definitions);
        }

        public static string[] SetUpAndCompile(List<AbstractPlugin> chain, params string[] fileNames)
        {
            return SetUpAndCompile(chain, new Settings(), new Definitions(), fileNames);
        }

        public static string[] SetUpAndCompile(List<AbstractPlugin> chain, IDefinitions definitions,
            params string[] fileNames)
        {

            return SetUpAndCompile(chain, new Settings(), definitions, fileNames);
        }

        public static string[] SetUpAndCompile(List<AbstractPlugin> chain, Settings settings, params string[] fileNames)
        {
            return SetUpAndCompile(chain, settings, new Definitions(), fileNames);
        }


    }
}