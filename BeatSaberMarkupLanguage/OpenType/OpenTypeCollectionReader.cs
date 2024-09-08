using System.IO;
using System.Text;

namespace BeatSaberMarkupLanguage.OpenType
{
    internal class OpenTypeCollectionReader : OpenTypeFontReader
    {
        public OpenTypeCollectionReader(Stream input)
            : base(input)
        {
        }

        public OpenTypeCollectionReader(Stream input, Encoding encoding)
            : base(input, encoding)
        {
        }

        public OpenTypeCollectionReader(Stream input, Encoding encoding, bool leaveOpen)
            : base(input, encoding, leaveOpen)
        {
        }

        public CollectionHeader ReadCollectionHeader()
        {
            CollectionHeader header = new()
            {
                TTCTag = ReadTag(),
                MajorVersion = ReadUInt16(),
                MinorVersion = ReadUInt16(),
                NumFonts = ReadUInt32(),
            };
            header.OffsetTable = new uint[header.NumFonts];
            for (uint i = 0; i < header.NumFonts; i++)
            {
                header.OffsetTable[i] = ReadOffset32();
            }

            if (header.MajorVersion == 2)
            {
                header.DSIGTag = ReadUInt32();
                header.DSIGLength = ReadUInt32();
                header.DSIGOffset = ReadUInt32();
            }

            return header;
        }

        public OffsetTable[] ReadOffsetTables(CollectionHeader header)
        {
            OffsetTable[] tables = new OffsetTable[header.NumFonts];
            for (uint i = 0; i < header.NumFonts; i++)
            {
                BaseStream.Position = header.OffsetTable[i];
                tables[i] = ReadOffsetTable();
            }

            return tables;
        }

        public OpenTypeFont[] ReadFonts(CollectionHeader header, bool lazyLoad = true)
        {
            OpenTypeFont[] fonts = new OpenTypeFont[header.NumFonts];
            for (uint i = 0; i < header.NumFonts; i++)
            {
                BaseStream.Position = header.OffsetTable[i];
                fonts[i] = new OpenTypeFont(this, lazyLoad);
            }

            return fonts;
        }
    }
}
