using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSaberMarkupLanguage.OpenType
{
    public class OpenTypeCollection : IEnumerable<OpenTypeFont>, IDisposable
    {
        private readonly CollectionHeader header;
        private OpenTypeFont[] fonts;
        private readonly bool lazy;

        public OpenTypeCollectionReader Reader { get; }

        public OpenTypeCollection(OpenTypeCollectionReader reader, bool lazyLoad = true) : this(reader.ReadCollectionHeader(), reader, lazyLoad)
        {
        }
        public OpenTypeCollection(CollectionHeader header, OpenTypeCollectionReader reader, bool lazyLoad = true)
        {
            this.header = header;
            lazy = lazyLoad;
            if (lazyLoad)
                Reader = reader;
            else
                LoadAll(reader);
        }

        private void LoadAll(OpenTypeCollectionReader reader)
        {
            fonts = ReadFonts(reader);
        }

        public IEnumerable<OpenTypeFont> Fonts
            => fonts ??= ReadFonts(Reader);

        private OpenTypeFont[] ReadFonts(OpenTypeCollectionReader reader)
            => reader.ReadFonts(header, lazy);

        public IEnumerator<OpenTypeFont> GetEnumerator()
            => Fonts.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => Fonts.GetEnumerator();

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~OpenTypeCollection()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
