using System;
using System.Collections.Generic;
using System.Linq;

namespace BeatSaberMarkupLanguage.OpenType
{
    public class OpenTypeFont : IDisposable
    {
        private readonly OffsetTable offsetTable;

        private readonly TableRecord[] tables;
        private readonly TableRecord? nameTableRecord;

        public OpenTypeFontReader Reader { get; }

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

        private void LoadAllTables(OpenTypeFontReader reader)
        {
            // TODO: do something with this
            nameTable = ReadNameTable(reader);
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
                    // because I'm a stupid little bitch I prefer US English
                    { PlatformID: OpenTypeNameTable.NameRecord.Platform.Windows, LanguageID: OpenTypeNameTable.NameRecord.USEnglishLangID } => 100,
                    _ => 0,
                };

            return NameTable.NameRecords.Where(r => r.NameID == type)
                .OrderByDescending(r => RankPlatform(r) + RankLanguage(r))
                .FirstOrDefault();
        }

        public IEnumerable<TableRecord> Tables => tables;

        private bool disposedValue = false;

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

        public void Dispose() => Dispose(true);
    }
}
