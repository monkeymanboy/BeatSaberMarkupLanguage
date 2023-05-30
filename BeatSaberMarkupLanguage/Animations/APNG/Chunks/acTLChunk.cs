using System.IO;

namespace BeatSaberMarkupLanguage.Animations.APNG.Chunks
{
    /// <summary>
    /// Animation Control chunk.
    /// </summary>
    internal class acTLChunk : Chunk
    {
        private uint frameCount;
        private uint playCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="acTLChunk"/> class.
        /// </summary>
        internal acTLChunk()
        {
            Length = 8;
            ChunkType = "acTL";
            ChunkData = new byte[Length];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="acTLChunk"/> class.
        /// </summary>
        /// <param name="bytes">Byte array of chunk data.</param>
        public acTLChunk(byte[] bytes)
            : base(bytes)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="acTLChunk"/> class.
        /// </summary>
        /// <param name="ms">Memory stream of chunk data.</param>
        public acTLChunk(MemoryStream ms)
            : base(ms)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="acTLChunk"/> class.
        /// </summary>
        /// <param name="chunk">Chunk object.</param>
        public acTLChunk(Chunk chunk)
            : base(chunk)
        {
        }

        /// <summary>
        /// Gets the number frames.
        /// </summary>
        /// <value>The number frames.</value>
        public uint FrameCount
        {
            get => frameCount;
            internal set
            {
                frameCount = value;
                ModifyChunkData(0, Helper.ConvertEndian(value));
            }
        }

        /// <summary>
        /// Gets the number plays.
        /// </summary>
        /// <value>The number plays.</value>
        public uint PlayCount
        {
            get => playCount;
            internal set
            {
                playCount = value;
                ModifyChunkData(4, Helper.ConvertEndian(value));
            }
        }

        /// <summary>
        /// Parses the data.
        /// </summary>
        /// <param name="ms">Memory stream to parse.</param>
        protected override void ParseData(MemoryStream ms)
        {
            frameCount = Helper.ConvertEndian(ms.ReadUInt32());
            playCount = Helper.ConvertEndian(ms.ReadUInt32());
        }
    }
}
