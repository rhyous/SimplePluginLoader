using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Rhyous.SimplePluginLoader
{


    public interface ITextWriter : IDisposable
    {
        void Write(string value);
        void Flush();
    }

    public class TextWriterWrapper : ITextWriter
    {
        private readonly TextWriter _TextWriter;
        private bool _DisposedValue;

        public TextWriterWrapper(TextWriter textWriter)
        {
            _TextWriter = textWriter;
        }

        public void Flush()
        {
            _TextWriter.Flush();
        }

        public void Write(string value)
        {
            _TextWriter.Write(value);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_DisposedValue)
            {
                if (disposing)
                {
                    _TextWriter.Dispose();
                }
                _DisposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
