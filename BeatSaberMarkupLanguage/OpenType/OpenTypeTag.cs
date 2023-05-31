using System;
using System.Linq;

namespace BeatSaberMarkupLanguage.OpenType
{
    public struct OpenTypeTag
    {
        public static readonly OpenTypeTag NAME = FromString("name");

        public OpenTypeTag(byte[] value) => Value = value;

        public byte[] Value { get; set; }

        public uint IntValue => BitConverter.ToUInt32(Value, 0);

        public static bool operator ==(OpenTypeTag left, OpenTypeTag right)
            => left.Equals(right);

        public static bool operator !=(OpenTypeTag left, OpenTypeTag right)
            => !(left == right);

        public static OpenTypeTag FromChars(char[] chrs)
            => new(chrs.Select(c => (byte)c).ToArray());

        public static OpenTypeTag FromString(string str)
            => FromChars(str.ToCharArray(0, 4));

        public bool Validate()
            => Value.Length == 4 && Value.All(b => b is >= 0x20 and <= 0x7E);

        public override bool Equals(object obj)
            => obj is OpenTypeTag tag && IntValue == tag.IntValue;

        public override int GetHashCode()
            => 1637310455 + IntValue.GetHashCode();
    }
}
