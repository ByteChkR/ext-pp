using System.Collections.Generic;
using ext_compiler;
using NUnit.Framework;

namespace ext_compiler.tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        

        [Test]
        public void IncludeCircular()
        {
            List<SourceScript> tree = ExtensionCompiler.LoadSourceTree("tests/includecircular.cl");
            Assert.AreEqual(tree.Count, 3);
            
        }

        [Test]
        public void IncludeGenericCircular()
        {
            List<SourceScript> tree = ExtensionCompiler.LoadSourceTree("tests/genericincludepassthrough.cl");
            Assert.AreEqual(tree.Count, 5);
        }

        [Test]
        public void TypePassing()
        {
            List<SourceScript> tree = ExtensionCompiler.LoadSourceTree("tests/typePassing.cl");
            Assert.AreEqual(tree.Count, 4);
        }
    }
}