using System.Collections.Generic;
using System.IO;
using System.Threading;
using ext_pp;
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
            MethodPrecompiler.PrecompileClass(typeof(ExtensionProcessor));
            MethodPrecompiler.PrecompileClass(typeof(SourceScript));
        }

        [Test]
        public void IncludeCircular()
        {
            bool ret = ExtensionProcessor.LoadSourceTree("includecircular.cl", out List<SourceScript> tree);
            Assert.AreEqual(
                tree.Count, 
                3);
            Assert.IsTrue(ret);
        }

        [Test]
        public void IncludeGenericCircular()
        {
            bool ret = ExtensionProcessor.LoadSourceTree("genericincludepassthrough.cl", out List<SourceScript> tree);
            Assert.AreEqual(
                tree.Count, 
                5);
            Assert.IsTrue(ret);
        }

        [Test]
        public void TypePassing()
        {
            bool ret = ExtensionProcessor.LoadSourceTree("typePassing.cl", out List<SourceScript> tree);
            Assert.AreEqual(
                tree.Count, 
                4);
            Assert.IsTrue(ret);
        }
    }
}