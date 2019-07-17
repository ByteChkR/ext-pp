using System;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using ADL;
using Utils = ext_pp_base.Utils;

namespace ext_pp_cli
{
    public class ArgumentHandler<T>
    {
        public delegate T[] Converter (string[] argIn, object defaul);

        private readonly Converter _converter = null;
        public readonly string Key;
        public ArgumentHandler(string key, Converter conv)
        {
            _converter = conv;
            this.Key = key;
        }


        public T[] GetValue(string[] val, object defaul)
        {
            return _converter(val, defaul);
        }

        
    }

    public class ArgumentHandler
    {
        public static ArgumentHandler<T> Create<T>(string key)
        {
            return new ArgumentHandler<T>(key, Utils.ParseArray<T>);
        }
    }
}