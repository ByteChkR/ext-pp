using System.Collections.Generic;
using System.IO;
using System.Threading;
using ext_compiler;
using NUnit.Framework;

namespace ext_compiler.tests
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
            Assert.AreEqual(
                ExtensionProcessor.LoadSourceTree("includecircular.cl").Count, 
                3);
            
        }

        [Test]
        public void IncludeGenericCircular()
        {
            Assert.AreEqual(
                ExtensionProcessor.LoadSourceTree("genericincludepassthrough.cl").Count, 
                5);
        }

        [Test]
        public void TypePassing()
        {
            Assert.AreEqual(
                ExtensionProcessor.LoadSourceTree("typePassing.cl").Count, 
                4);
        }
    }
}