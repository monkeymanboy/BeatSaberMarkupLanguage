using System.IO;

namespace BeatSaberMarkupLanguage.Animations.Chunks
{
    /// <summary>
    /// IEND chunk representing the end of the PNG
    /// </summary>
    internal class IENDChunk : Chunk
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnimatedImages.IENDChunk"/> class.
        /// </summary>
        /// <param name="bytes">Bytes array representation.</param>
        public IENDChunk(byte[] bytes)
            : base(bytes)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnimatedImages.IENDChunk"/> class.
        /// </summary>
        /// <param name="ms">Memory Stream representation..</param>
        public IENDChunk(MemoryStream ms)
            : base(ms)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnimatedImages.IENDChunk"/> class.
        /// </summary>
        /// <param name="chunk">Chunk representation.</param>
        public IENDChunk(Chunk chunk)
            : base(chunk)
        {
        }
    }
}