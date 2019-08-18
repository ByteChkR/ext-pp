using System;
using System.Collections.Generic;

namespace ext_pp_base
{
    public interface IChainCollection
    {
        List<Type> Chain { get; }
        string Name { get; }
    }
}