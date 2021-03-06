﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using ext_pp;
using ext_pp_base;
using ext_pp_base.settings;

namespace ext_pp_tests
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

        private static PreProcessor SetUp(List<AbstractPlugin> chain)
        {
            PreProcessor pp = new PreProcessor();
            pp.SetFileProcessingChain(chain);
            return pp;
        }

        public static ISourceScript[] SetUpAndProcess(List<AbstractPlugin> chain, Settings settings, IDefinitions definitions, params string[] fileNames)
        {

            PreProcessor pp = SetUp(chain);
            return pp.ProcessFiles(fileNames.Select(x=>new FilePathContent(Path.GetFullPath(x))).OfType<IFileContent>().ToArray(), settings, definitions);
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
            PreProcessor pp = SetUp(chain);
            
            return pp.Run(fileNames.Select(x=>new FilePathContent(x)).OfType<IFileContent>().ToArray(), settings, definitions);
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