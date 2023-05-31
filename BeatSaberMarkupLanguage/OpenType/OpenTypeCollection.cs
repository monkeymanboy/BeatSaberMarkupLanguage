using System;
using System.Collections;
using System.Collections.Generic;

namespace BeatSaberMarkupLanguage.OpenType
{
    public class OpenTypeCollection : IEnumerable<OpenTypeFont>, IDisposable
    {
        private readonly CollectionHeader header;
        private readonly bool lazy;
        private OpenTypeFont[] fonts;

        private bool disposedValue = false;

        public OpenTypeCollection(OpenTypeCollectionReader reader, bool lazyLoad = true)
            : this(reader.ReadCollectionHeader(), reader, lazyLoad)
        {
        }

        public OpenTypeCollection(CollectionHeader header, OpenTypeCollectionReader reader, bool lazyLoad = true)
        {
            this.header = header;
            lazy = lazyLoad;
            if (lazyLoad)
            {
                Reader = reader;
            }
            else
            {
                LoadAll(reader);
            }
        }

        public OpenTypeCollectionReader Reader { get; }

        public IEnumerable<OpenTypeFont> Fonts
            => fonts ??= ReadFonts(Reader);

        public IEnumerator<OpenTypeFont> GetEnumerator()
            => Fonts.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => Fonts.GetEnumerator();

        public void Dispose() => Dispose(true);

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                disposedValue = true;
            }
        }

        private void LoadAll(OpenTypeCollectionReader reader)
        {
            fonts = ReadFonts(reader);
        }

        private OpenTypeFont[] ReadFonts(OpenTypeCollectionReader reader)
            => reader.ReadFonts(header, lazy);
    }
}
