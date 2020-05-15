using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSaberMarkupLanguage.OpenType
{
    public struct TableRecord
    {
        public OpenTypeTag TableTag { get; set; }
        public uint Checksum { get; set; }
        /// <summary>
        /// Offset from the beginning of the file.
        /// </summary>
        public uint Offset { get; set; }
        public uint Length { get; set; }

        public bool Validate() => TableTag.Validate();
    }
}
