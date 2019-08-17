using System;

namespace ext_pp_base
{
    public class ProcessorException : ApplicationException
    {
        public ProcessorException(string message) : base(message) { }
    }
}