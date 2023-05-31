using System;
using System.IO;
using System.Text;

namespace BeatSaberMarkupLanguage.OpenType
{
    public abstract class OpenTypeReader : BinaryReader
    {
        protected OpenTypeReader(Stream input)
            : base(input)
        {
        }

        protected OpenTypeReader(Stream input, Encoding encoding)
            : base(input, encoding)
        {
        }

        protected OpenTypeReader(Stream input, Encoding encoding, bool leaveOpen)
            : base(input, encoding, leaveOpen)
        {
        }

        public static byte[] FromBigEndian(byte[] bytes)
        {
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }

            return bytes;
        }

        public static OpenTypeReader For(Stream stream, Encoding enc = null, bool leaveOpen = false)
        {
            enc ??= Encoding.Default;

            var start = stream.Position;
            using var reader = new BinaryReader(stream, enc, true);
            var tag = BitConverter.ToUInt32(FromBigEndian(reader.ReadBytes(4)), 0);
            stream.Position = start;

            return tag switch
            {
                CollectionHeader.TTCTagBEInt => new OpenTypeCollectionReader(stream, enc, leaveOpen),
                OffsetTable.OpenTypeCFFVersion => new OpenTypeFontReader(stream, enc, leaveOpen),
                OffsetTable.TrueTypeOnlyVersion => new OpenTypeFontReader(stream, enc, leaveOpen),
                _ => null,
            };
        }

        public byte ReadUInt8() => ReadByte();

        public sbyte ReadInt8() => ReadSByte();

        public new ushort ReadUInt16()
            => BitConverter.ToUInt16(FromBigEndian(ReadBytes(2)), 0);

        public new short ReadInt16()
            => BitConverter.ToInt16(FromBigEndian(ReadBytes(2)), 0);

        public new uint ReadUInt32()
            => BitConverter.ToUInt32(FromBigEndian(ReadBytes(4)), 0);

        public new int ReadInt32()
            => BitConverter.ToInt32(FromBigEndian(ReadBytes(4)), 0);

        public int ReadFixed() => ReadInt32();

        public short ReadFWord() => ReadInt16();

        public ushort ReadUFWord() => ReadUInt16();

        public short ReadF2Dot14() => ReadInt16();

        public long ReadLongDateTime()
            => BitConverter.ToInt64(FromBigEndian(ReadBytes(8)), 0);

        public OpenTypeTag ReadTag() => new(ReadBytes(4));

        public ushort ReadOffset16() => ReadUInt16();

        public uint ReadOffset32() => ReadUInt32();
    }
}
