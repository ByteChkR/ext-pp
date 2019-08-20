using System.Collections.Generic;
using System.IO;
using System.Net.Mime;
using ext_pp_base;
using ext_pp_plugins;
using NUnit.Framework;

namespace ext_pp.tests
{
    public static class Base64BlockPluginTests
    {
        private static string ResourceFolder { get; } = TestHelper.ResourceFolder + "B64_tests/";

        [SetUp]
        public static void SetUp()
        {
            TestHelper.SetupPath();
        }

        [Test]
        public static void Base64BlockDecodeTest()
        {
            Directory.SetCurrentDirectory(ResourceFolder);
            var ret = TestHelper.SetUpAndProcess(new List<AbstractPlugin> { new Base64BlockDecoder() }, "decode_test.txt");
            Assert.IsTrue(ret[0].GetSource()[0] == "HelloWASAAAAAAAAAAAABI");
        }



        [Test]
        public static void Base64BlockEncodeTest()
        {
            Directory.SetCurrentDirectory(ResourceFolder);
            var ret = TestHelper.SetUpAndProcess(new List<AbstractPlugin> { new Base64BlockDecoder() }, "encode_test.txt");
            Assert.IsTrue(ret[0].GetSource()[0] == "SGVsbG9XQVNBQUFBQUFBQUFBQUFCSQ==");
        }
    }
}