using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSaberMarkupLanguage.OpenType
{
    public class OpenTypeFont : IDisposable
    {
        private readonly OffsetTable offsetTable;

        private readonly TableRecord[] tables;
        private readonly TableRecord? nameTableRecord;

        public OpenTypeFontReader Reader { get; }

        public OpenTypeFont(OpenTypeFontReader reader, bool lazyLoad = true) : this(reader.ReadOffsetTable(), reader, lazyLoad)
        {
        }

        public OpenTypeFont(OffsetTable offsets, OpenTypeFontReader reader, bool lazyLoad = true)
        {
            offsetTable = offsets;
            tables = reader.ReadTableRecords(offsetTable);
            nameTableRecord = tables.Select(t => new TableRecord?(t))
                .FirstOrDefault(t => t.Value.TableTag == OpenTypeTag.NAME);

            if (lazyLoad)
                Reader = reader;
            else
                LoadAllTables(reader);
        }

        private void LoadAllTables(OpenTypeFontReader reader)
        {
            nameTable = ReadNameTable(reader);
            // TODO: do something with this
        }

        private OpenTypeNameTable nameTable = null;
        public OpenTypeNameTable NameTable
            => nameTable ??= ReadNameTable(Reader);

        private string uniqueId = null;
        public string UniqueId
            => uniqueId ??= FindBestNameRecord(OpenTypeNameTable.NameRecord.NameType.UniqueId)?.Value;

        private string family = null;
        public string Family
            => family ??= FindBestNameRecord(OpenTypeNameTable.NameRecord.NameType.FontFamily)?.Value;

        private string subfamily = null;
        public string Subfamily
            => subfamily ??= FindBestNameRecord(OpenTypeNameTable.NameRecord.NameType.FontSubfamily)?.Value;

        private string fullName = null;
        public string FullName
            => fullName ??= FindBestNameRecord(OpenTypeNameTable.NameRecord.NameType.FullFontName)?.Value;

        private OpenTypeNameTable ReadNameTable(OpenTypeFontReader reader)
            => reader.TryReadTable(nameTableRecord.Value) as OpenTypeNameTable;

        private OpenTypeNameTable.NameRecord FindBestNameRecord(OpenTypeNameTable.NameRecord.NameType type)
        {
            static int RankPlatform(OpenTypeNameTable.NameRecord record)
                => record.PlatformID switch
                {
                    OpenTypeNameTable.NameRecord.Platform.Windows => 3000,
                    OpenTypeNameTable.NameRecord.Platform.Unicode => 2000,
                    OpenTypeNameTable.NameRecord.Platform.Macintosh => 1000,
                    _ => 0,
                };
            static int RankLanguage(OpenTypeNameTable.NameRecord record)
                => record switch
                {
                    { // because i'm a stupid little bitch i prefer US English
                        PlatformID: OpenTypeNameTable.NameRecord.Platform.Windows,
                        LanguageID: OpenTypeNameTable.NameRecord.USEnglishLangID
                    } => 100,
                    _ => 0,
                };

            return NameTable.NameRecords.Where(r => r.NameID == type)
                .OrderByDescending(r => RankPlatform(r) + RankLanguage(r))
                .FirstOrDefault();
        }

        public IEnumerable<TableRecord> Tables => tables;

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Reader?.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~OpenTypeFont()
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
