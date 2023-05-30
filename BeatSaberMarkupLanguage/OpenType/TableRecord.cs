namespace BeatSaberMarkupLanguage.OpenType
{
    public struct TableRecord
    {
        public OpenTypeTag TableTag { get; set; }

        public uint Checksum { get; set; }

        /// <summary>
        /// Gets or sets the offset from the beginning of the file.
        /// </summary>
        public uint Offset { get; set; }

        public uint Length { get; set; }

        public bool Validate() => TableTag.Validate();
    }
}
