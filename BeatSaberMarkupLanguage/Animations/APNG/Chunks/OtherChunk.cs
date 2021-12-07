using System.IO;

namespace BeatSaberMarkupLanguage.Animations.APNG.Chunks
{
    /// <summary>
    /// Other PNG chunks.
    /// </summary>
    internal class OtherChunk : Chunk
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OtherChunk"/> class.
        /// </summary>
        /// <param name="bytes">Byte Array of chunk data.</param>
        public OtherChunk(byte[] bytes)
            : base(bytes)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OtherChunk"/> class.
        /// </summary>
        /// <param name="ms">Memory Stream of chunk data.</param>
        public OtherChunk(MemoryStream ms)
            : base(ms)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OtherChunk"/> class.
        /// </summary>
        /// <param name="chunk">Chunk data.</param>
        public OtherChunk(Chunk chunk)
            : base(chunk)
        {
        }

        /// <summary>
        /// Parses the data.
        /// </summary>
        /// <param name="ms">Memory Stream of chunk data.</param>
        protected override void ParseData(MemoryStream ms)
        {
        }
    }
}