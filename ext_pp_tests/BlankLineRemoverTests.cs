using System.Collections.Generic;
using System.IO;
using ext_pp_base;
using ext_pp_plugins;
using NUnit.Framework;

namespace ext_pp_tests
{
    public static class BlankLineRemoverTests
    {
        private static string ResourceFolder { get; } = TestHelper.ResourceFolder + "BLR_tests/";

        [SetUp]
        public static void SetUp()
        {
            TestHelper.SetupPath();
        }

        [Test]
        public static void BlankLineRemoverTest()
        {
            Directory.SetCurrentDirectory(ResourceFolder);
            var ret = TestHelper.SetUpAndCompile(new List<AbstractPlugin> { new BlankLineRemover() }, "blankline_test.txt");
            Assert.IsTrue(ret.Length == 0);
        }

    }
}