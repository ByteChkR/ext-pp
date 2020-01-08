using System.Collections.Generic;
using System.IO;
using ext_pp_base;
using ext_pp_plugins;
using NUnit.Framework;

namespace ext_pp_tests
{
    public static class TextEncoderPluginTests
    {
        private static string ResourceFolder { get; } = TestHelper.ResourceFolder + "TENC_tests/";

        [SetUp]
        public static void SetUp()
        {
            TestHelper.SetupPath();
        }

        [Test]
        public static void Base64BlockDecodeTest()
        {
            Directory.SetCurrentDirectory(ResourceFolder);
            var ret = TestHelper.SetUpAndProcess(new List<AbstractPlugin> { new TextEncoderPlugin() }, "decode_b64_test.txt");
            Assert.IsTrue(ret[0].GetSource()[0] == "HelloWASAAAAAAAAAAAABI");
        }



        [Test]
        public static void Base64BlockEncodeTest()
        {
            Directory.SetCurrentDirectory(ResourceFolder);
            var ret = TestHelper.SetUpAndProcess(new List<AbstractPlugin> { new TextEncoderPlugin() }, "encode_b64_test.txt");
            Assert.IsTrue(ret[0].GetSource()[0] == "SGVsbG9XQVNBQUFBQUFBQUFBQUFCSQ==");
        }

        [Test]
        public static void ROTBlockDecodeTest()
        {
            Directory.SetCurrentDirectory(ResourceFolder);
            var ret = TestHelper.SetUpAndProcess(new List<AbstractPlugin> { new TextEncoderPlugin() }, "decode_rot_test.txt");
            Assert.IsTrue(ret[0].GetSource()[0] == "Hello WASAAAAAAAAAAAABIZZ");
        }



        [Test]
        public static void ROTBlockEncodeTest()
        {
            Directory.SetCurrentDirectory(ResourceFolder);
            var ret = TestHelper.SetUpAndProcess(new List<AbstractPlugin> { new TextEncoderPlugin() }, "encode_rot_test.txt");
            Assert.IsTrue(ret[0].GetSource()[0] == "Ifmmp XBTBBBBBBBBBBBBCJAA");
        }
    }
}