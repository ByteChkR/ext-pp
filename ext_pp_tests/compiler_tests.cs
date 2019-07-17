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
    public class Tests
    {
        public static string ResourceFolder = Path.GetFullPath("../../../res/");

        [SetUp]
        public void Setup()
        {
            Directory.SetCurrentDirectory(ResourceFolder);
            //MethodPrecompiler.PrecompileClass(typeof(ExtensionProcessor));
            //MethodPrecompiler.PrecompileClass(typeof(SourceScript));
        }

        [Test]
        public void IncludeCircular()
        {

            PreProcessor pp = new PreProcessor();
            List<AbstractPlugin> lp = new List<AbstractPlugin>()
            {
                new IncludePlugin(),
            };
            pp.SetFileProcessingChain(lp);
            var ret = pp.Process(new[] { "includecircular.cl" }, new Settings(), new Definitions());
            Assert.AreEqual(
                ret.Length,
                3);
        }

        [Test]
        public void IncludeGenericCircular()
        {
            PreProcessor pp = new PreProcessor();

            List<AbstractPlugin> lp = new List<AbstractPlugin>()
            {
                new FakeGenericsPlugin(),
                new IncludePlugin(),
            };

            
            pp.SetFileProcessingChain(lp);
            var ret = pp.Process(new[] { "genericincludepassthrough.cl" }, new Settings(), new Definitions());
            Assert.AreEqual(
                ret.Length,
                5);
        }

        [Test]
        public void TypePassing()
        {
            PreProcessor pp = new PreProcessor();
            List<AbstractPlugin> lp = new List<AbstractPlugin>()
            {
                new FakeGenericsPlugin(),
                new IncludePlugin(),
            };
            pp.SetFileProcessingChain(lp);
            var ret = pp.Process(new[] { "typePassing.cl" }, new Settings(), new Definitions());
            Assert.AreEqual(
                ret.Length,
                4);
        }
    }
}