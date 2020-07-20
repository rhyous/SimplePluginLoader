using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Rhyous.SimplePluginLoader
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    internal class RuntimePluginLoaderException : Exception
    {
        public RuntimePluginLoaderException()
        {
        }

        public RuntimePluginLoaderException(string message) : base(message)
        {
        }

        public RuntimePluginLoaderException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected RuntimePluginLoaderException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}