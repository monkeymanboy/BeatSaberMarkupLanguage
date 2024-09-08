using System;
using System.Collections.Generic;
using System.Linq;

namespace BeatSaberMarkupLanguage.OpenType
{
    internal class OpenTypeFont : IDisposable
    {
        private readonly OffsetTable offsetTable;

        private readonly TableRecord[] tables;
        private readonly TableRecord? nameTableRecord;

        private OpenTypeNameTable nameTable = null;
        private string uniqueId = null;
        private string family = null;
        private string subfamily = null;
        private string fullName = null;

        private bool disposedValue = false;

        public OpenTypeFont(OpenTypeFontReader reader, bool lazyLoad = true)
            : this(reader.ReadOffsetTable(), reader, lazyLoad)
        {
        }

        public OpenTypeFont(OffsetTable offsets, OpenTypeFontReader reader, bool lazyLoad = true)
        {
            offsetTable = offsets;
            tables = reader.ReadTableRecords(offsetTable);
            nameTableRecord = tables.Select(t => new TableRecord?(t))
                .Where(t => t.Value.TableTag == OpenTypeTag.NAME).FirstOrDefault();

            if (lazyLoad)
            {
                Reader = reader;
            }
            else
            {
                LoadAllTables(reader);
            }
        }

        public OpenTypeFontReader Reader { get; }

        public OpenTypeNameTable NameTable
            => nameTable ??= ReadNameTable(Reader);

        public IEnumerable<TableRecord> Tables => tables;

        public string UniqueId
            => uniqueId ??= FindBestNameRecord(OpenTypeNameTable.NameRecord.NameType.UniqueId)?.Value;

        public string Family
            => family ??= FindBestNameRecord(OpenTypeNameTable.NameRecord.NameType.FontFamily)?.Value;

        public string Subfamily
            => subfamily ??= FindBestNameRecord(OpenTypeNameTable.NameRecord.NameType.FontSubfamily)?.Value;

        public string FullName
            => fullName ??= FindBestNameRecord(OpenTypeNameTable.NameRecord.NameType.FullFontName)?.Value;

        public void Dispose() => Dispose(true);

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Reader?.Dispose();
                }

                disposedValue = true;
            }
        }

        private void LoadAllTables(OpenTypeFontReader reader)
        {
            // TODO: do something with this
            nameTable = ReadNameTable(reader);
        }

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
                    // because I'm a stupid little bitch I prefer US English
                    { PlatformID: OpenTypeNameTable.NameRecord.Platform.Windows, LanguageID: OpenTypeNameTable.NameRecord.USEnglishLangID } => 100,
                    _ => 0,
                };

            return NameTable.NameRecords.Where(r => r.NameID == type)
                .OrderByDescending(r => RankPlatform(r) + RankLanguage(r))
                .FirstOrDefault();
        }
    }
}
