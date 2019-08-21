using System;

namespace ext_pp_base
{
    /// <summary>
    /// A processor exception is thrown when one of the ext_pp projects encounters an unresolvable error
    /// </summary>
    public class ProcessorException : ApplicationException
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Message providing info about the crash</param>
        public ProcessorException(string message) : base(message) { }
    }
}