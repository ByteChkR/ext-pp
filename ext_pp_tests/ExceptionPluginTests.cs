using System;
using System.Collections.Generic;
using System.IO;
using ext_pp_base;
using ext_pp_base.settings;
using ext_pp_plugins;
using NUnit.Framework;

namespace ext_pp.tests
{
    public static class ExceptionPluginTests
    {

        private static string ResourceFolder { get; } = TestHelper.ResourceFolder + "EX_tests/";


        [SetUp]
        public static void SetUp()
        {
            TestHelper.SetupPath();
        }

        [Test]
        public static void ExceptionWarningTest()
        {
            Directory.SetCurrentDirectory(ResourceFolder);
            Settings s = new Settings(new Dictionary<string, string[]>
            {
                {"-ex:tow", new string[0]}
            });
            try
            {
                TestHelper.SetUpAndProcess(new List<AbstractPlugin> { new ExceptionPlugin() }, s, "warning_test.txt");
                Assert.Fail();
            }
            catch (Exception)
            {
                //It will throw if it works.
            }
        }
        [Test]
        public static void ExceptionErrorTest()
        {
            Directory.SetCurrentDirectory(ResourceFolder);
            
            try
            {
                TestHelper.SetUpAndProcess(new List<AbstractPlugin> { new ExceptionPlugin() }, "error_test.txt");
                Assert.Fail();
            }
            catch (Exception)
            {
                //It will throw if it works.
            }
        }
    }
}