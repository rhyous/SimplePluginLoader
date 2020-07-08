using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Rhyous.SimplePluginLoader
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    internal class PluginPathUndefinedException : Exception
    {
        public PluginPathUndefinedException()
        {
        }

        public PluginPathUndefinedException(string message) : base(message)
        {
        }

        public PluginPathUndefinedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected PluginPathUndefinedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}