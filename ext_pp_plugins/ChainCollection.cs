using System;
using System.Collections.Generic;
using ext_pp_base;

namespace ext_pp_plugins
{
    public class ChainCollection : IChainCollection
    {
        public string Name { get; } = "NO_NAME";

        public List<Type> Chain => new List<Type>
            {
                typeof(MultiLinePlugin),
                typeof(KeyWordReplacer),
                typeof(ConditionalPlugin),
                typeof(FakeGenericsPlugin),
                typeof(IncludePlugin),
                typeof(WarningPlugin),
                typeof(ErrorPlugin),
                typeof(BlankLineRemover),
            };

    }
}