using System;
using System.Collections;
using System.Collections.Generic;

namespace BeatSaberMarkupLanguage.OpenType
{
    public class OpenTypeCollection : IEnumerable<OpenTypeFont>, IDisposable
    {
        private readonly CollectionHeader header;
        private OpenTypeFont[] fonts;
        private readonly bool lazy;

        public OpenTypeCollectionReader Reader { get; }

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

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                disposedValue = true;
            }
        }

        public void Dispose() => Dispose(true);
    }
}
