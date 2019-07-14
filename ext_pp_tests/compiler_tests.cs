using System.Collections.Generic;
using System.IO;
using System.Threading;
using ext_pp;
using ext_pp.settings;
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
            PreProcessor pp = new PreProcessor(new Settings());
            var ret = pp.Process("includecircular.cl", new Definitions());
            Assert.AreEqual(
                ret.Length, 
                3);
        }

        [Test]
        public void IncludeGenericCircular()
        {
            PreProcessor pp = new PreProcessor(new Settings());
            var ret = pp.Process("genericincludepassthrough.cl", new Definitions());
            Assert.AreEqual(
                ret.Length, 
                5);
        }

        [Test]
        public void TypePassing()
        {
            PreProcessor pp = new PreProcessor(new Settings());
            var ret = pp.Compile("typePassing.cl", new Definitions());
            Assert.AreEqual(
                ret.Length, 
                4);
        }
    }
}