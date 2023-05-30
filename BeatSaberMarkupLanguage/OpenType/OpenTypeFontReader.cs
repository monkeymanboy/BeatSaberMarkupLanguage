using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSaberMarkupLanguage.OpenType
{

    // TODO: this shit is a mess, clean it up
    public class OpenTypeFontReader : OpenTypeReader
    {
        public OpenTypeFontReader(Stream input) : base(input)
        {
        }

        public OpenTypeFontReader(Stream input, Encoding encoding) : base(input, encoding)
        {
        }

        public OpenTypeFontReader(Stream input, Encoding encoding, bool leaveOpen) : base(input, encoding, leaveOpen)
        {
        }

        public OffsetTable ReadOffsetTable()
            => new OffsetTable()
            {
                SFNTVersion = ReadUInt32(),
                NumTables = ReadUInt16(),
                SearchRange = ReadUInt16(),
                EntrySelector = ReadUInt16(),
                RangeShift = ReadUInt16(),
                TablesStart = BaseStream.Position,
            };

        protected TableRecord ReadTableRecord()
            => new TableRecord()
            {
                TableTag = ReadTag(),
                Checksum = ReadUInt32(),
                Offset = ReadOffset32(),
                Length = ReadUInt32()
            };

        public TableRecord[] ReadTableRecords(OffsetTable offsets)
        {
            BaseStream.Position = offsets.TablesStart;
            var tables = new TableRecord[offsets.NumTables];
            for (int i = 0; i < offsets.NumTables; i++)
                tables[i] = ReadTableRecord();
            return tables;
        }

        public TableRecord[] ReadAllTables() => ReadTableRecords(ReadOffsetTable());

        public OpenTypeTable TryReadTable(TableRecord table)
        {
            BaseStream.Position = table.Offset;

            OpenTypeTable result = null;
            if (table.TableTag == OpenTypeTag.NAME)
                result = new OpenTypeNameTable();

            result?.ReadFrom(this, table.Length);

            return result;
        }
    }
}
