using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ext_pp_base;
using ext_pp_base.settings;
using ext_pp_plugins;
using NUnit.Framework;

namespace ext_pp.tests
{
    public static class IncludeFakeGenericsTests
    {
        private static string ResourceFolder { get; } = TestHelper.ResourceFolder + "compiler_tests/";
        [SetUp]
        public static void SetUp()
        {
            TestHelper.SetupPath();
        }

        [Test]
        public static void IncludeCircular()
        {
            Directory.SetCurrentDirectory(ResourceFolder);
            var ret = TestHelper.SetUpAndProcess(new List<AbstractPlugin> { new IncludePlugin() }, new[] { "includecircular.cl" });

            Assert.AreEqual(
                ret.Length,
                3);
        }

        [Test]
        public static void IncludeGenericCircular()
        {
            Directory.SetCurrentDirectory(ResourceFolder);
            var ret = TestHelper.SetUpAndProcess(new List<AbstractPlugin> { new FakeGenericsPlugin(), new IncludePlugin(), }, new[] { "genericincludepassthrough.cl" });
            Assert.AreEqual(
                ret.Length,
                5);
        }

        [Test]
        public static void TypePassing()
        {
            Directory.SetCurrentDirectory(ResourceFolder);
            var ret = TestHelper.SetUpAndProcess(new List<AbstractPlugin> { new FakeGenericsPlugin(), new IncludePlugin(), }, new[] { "typePassing.cl" });

            Assert.AreEqual(
                ret.Length,
                4);
        }
    }
}