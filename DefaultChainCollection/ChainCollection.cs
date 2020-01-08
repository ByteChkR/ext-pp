using System;
using System.Collections.Generic;
using ext_pp_base;
using ext_pp_base.settings;

namespace ext_pp_plugins
{
    public class ChainCollection : IChainCollection
    {
        public string Name { get; } = "Default";

        public List<Type> Chain { get; } = new List<Type>
        {
            typeof(MultiLinePlugin),
            typeof(KeyWordReplacer),
            typeof(ConditionalPlugin),
            typeof(FakeGenericsPlugin),
            typeof(IncludePlugin),
            typeof(ExceptionPlugin),
            typeof(BlankLineRemover),
            typeof(ChangeCharCase),
            typeof(TextEncoderPlugin),
        };
    }
}