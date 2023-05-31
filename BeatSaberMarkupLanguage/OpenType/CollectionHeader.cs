namespace BeatSaberMarkupLanguage.OpenType
{
    public struct CollectionHeader
    {
        public const uint TTCTagBEInt = 0x74746366;
        public const uint DSIGTagValue = 0x44534947;

        public static readonly OpenTypeTag TTCExpectedTag = OpenTypeTag.FromString("ttcf");

        public OpenTypeTag TTCTag { get; set; }

        public ushort MajorVersion { get; set; }

        public ushort MinorVersion { get; set; }

        public uint NumFonts { get; set; }

        public uint[] OffsetTable { get; set; }

        public uint DSIGTag { get; set; }

        public uint DSIGLength { get; set; }

        public uint DSIGOffset { get; set; }
    }
}
