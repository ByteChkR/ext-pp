using System;
using System.Collections.Generic;

namespace ext_pp_base.settings
{
    /// <summary>
    /// An interface that can be inherited and then used to create predetermined collections of plugins.
    /// </summary>
    public interface IChainCollection
    {
        /// <summary>
        /// The chain of the collection
        /// </summary>
        List<Type> Chain { get; }

        /// <summary>
        /// The name to be found in the assembly.
        /// </summary>
        string Name { get; }
    }
}