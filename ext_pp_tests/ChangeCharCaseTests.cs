using System.Collections.Generic;
using System.IO;
using ext_pp_base;
using ext_pp_base.settings;
using ext_pp_plugins;
using NUnit.Framework;

namespace ext_pp.tests
{
    public static class ChangeCharCaseTests
    {

        private static string ResourceFolder { get; } = TestHelper.ResourceFolder + "CCC_tests/";

        [SetUp]
        public static void SetUp()
        {
            TestHelper.SetupPath();
        }

        [Test]
        public static void ChangeCharCaseToLowerTest()
        {
            Directory.SetCurrentDirectory(ResourceFolder);
            var ret = TestHelper.SetUpAndCompile(new List<AbstractPlugin> { new ChangeCharCase() }, "tolower_test.txt");
            Assert.IsTrue(ret[0]== "hello_this_works right?");
        }

        [Test]
        public static void ChangeCharCaseToUpperTest()
        {
            Directory.SetCurrentDirectory(ResourceFolder);
            var ret = TestHelper.SetUpAndCompile(
                new List<AbstractPlugin> { new ChangeCharCase() }, 
                new Settings(new Dictionary<string, string[]>
                {
                    {"-ccc:sc", new []{"toupper"} }
                }),
                "toupper_test.txt");
            Assert.IsTrue(ret[0] == "HELLO_THIS_WORKS RIGHT?");
        }
    }
}