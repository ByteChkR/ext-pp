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
            
            PreProcessor pp = new PreProcessor(new Settings());
            List<IPlugin> lp = new Dictionary<Type, IPlugin>()
            {
                {typeof(FakeGenericsPlugin), new FakeGenericsPlugin(new Settings())},
                {typeof(ConditionalPlugin), new ConditionalPlugin(new Settings())},
                {typeof(IncludePlugin), new IncludePlugin(new Settings())},
                {typeof(WarningPlugin), new WarningPlugin(new Settings())},
                {typeof(ErrorPlugin), new ErrorPlugin(new Settings())}
            }.Values.ToList();
            pp.SetFileProcessingChain(lp);
            var ret = pp.Process("includecircular.cl", new Definitions());
            Assert.AreEqual(
                ret.Length,
                3);
        }

        [Test]
        public void IncludeGenericCircular()
        {
            PreProcessor pp = new PreProcessor(new Settings());
            List<IPlugin> lp = new Dictionary<Type, IPlugin>()
            {
                {typeof(FakeGenericsPlugin), new FakeGenericsPlugin(new Settings())},
                {typeof(ConditionalPlugin), new ConditionalPlugin(new Settings())},
                { typeof(IncludePlugin), new IncludePlugin(new Settings())},
                { typeof(WarningPlugin), new WarningPlugin(new Settings())},
                { typeof(ErrorPlugin), new ErrorPlugin(new Settings())}
            }.Values.ToList();
            pp.SetFileProcessingChain(lp);
            var ret = pp.Process("genericincludepassthrough.cl", new Definitions());
            Assert.AreEqual(
                ret.Length,
                5);
        }

        [Test]
        public void TypePassing()
        {
            PreProcessor pp = new PreProcessor(new Settings());
            List<IPlugin> lp = new Dictionary<Type, IPlugin>()
            {
                {typeof(FakeGenericsPlugin), new FakeGenericsPlugin(new Settings())},
                {typeof(ConditionalPlugin), new ConditionalPlugin(new Settings())},
                { typeof(IncludePlugin), new IncludePlugin(new Settings())},
                { typeof(WarningPlugin), new WarningPlugin(new Settings())},
                { typeof(ErrorPlugin), new ErrorPlugin(new Settings())}
            }.Values.ToList();
            pp.SetFileProcessingChain(lp);
            var ret = pp.Process("typePassing.cl", new Definitions());
            Assert.AreEqual(
                ret.Length,
                4);
        }
    }
}