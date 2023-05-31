using System;
using System.IO;

namespace BeatSaberMarkupLanguage.Animations.APNG.Chunks
{
    /// <summary>
    /// PNG Image header chunk.
    /// </summary>
    internal class IHDRChunk : Chunk
    {
        private int width;
        private int height;
        private byte bitDepth;
        private byte colorType;
        private byte compressionMethod;
        private byte filterMethod;
        private byte interlaceMethod;

        /// <summary>
        /// Initializes a new instance of the <see cref="IHDRChunk"/> class.
        /// </summary>
        /// <param name="chunkBytes">Byte Array representation.</param>
        public IHDRChunk(byte[] chunkBytes)
            : base(chunkBytes)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IHDRChunk"/> class.
        /// </summary>
        /// <param name="ms">Memory stream representation.</param>
        public IHDRChunk(MemoryStream ms)
            : base(ms)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IHDRChunk"/> class.
        /// </summary>
        /// <param name="chunk">Chunk representation.</param>
        public IHDRChunk(Chunk chunk)
            : base(chunk)
        {
        }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
        public int Width
        {
            get => this.width;
            internal set
            {
                this.width = value;
                ModifyChunkData(0, BitConverter.GetBytes(Helper.ConvertEndian(value)));
            }
        }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>The height.</value>
        public int Height
        {
            get => this.height;
            internal set
            {
                this.height = value;
                ModifyChunkData(4, BitConverter.GetBytes(Helper.ConvertEndian(value)));
            }
        }

        /// <summary>
        /// Gets or sets the bit depth.
        /// </summary>
        /// <value>The bit depth.</value>
        public byte BitDepth
        {
            get => this.bitDepth;
            internal set
            {
                this.bitDepth = value;
                ModifyChunkData(5, new[] { value });
            }
        }

        /// <summary>
        /// Gets or sets the type of the color.
        /// </summary>
        /// <value>The type of the color.</value>
        public byte ColorType
        {
            get => this.colorType;
            internal set
            {
                this.colorType = value;
                ModifyChunkData(6, new[] { value });
            }
        }

        /// <summary>
        /// Gets or sets the compression method.
        /// </summary>
        /// <value>The compression method.</value>
        public byte CompressionMethod
        {
            get => this.compressionMethod;
            internal set
            {
                this.compressionMethod = value;
                ModifyChunkData(7, new[] { value });
            }
        }

        /// <summary>
        /// Gets or sets the filter method.
        /// </summary>
        /// <value>The filter method.</value>
        public byte FilterMethod
        {
            get => this.filterMethod;
            internal set
            {
                this.filterMethod = value;
                ModifyChunkData(8, new[] { value });
            }
        }

        /// <summary>
        /// Gets or sets the interlace method.
        /// </summary>
        /// <value>The interlace method.</value>
        public byte InterlaceMethod
        {
            get => this.interlaceMethod;
            internal set
            {
                this.interlaceMethod = value;
                ModifyChunkData(9, new[] { value });
            }
        }

        /// <summary>
        /// Parses the data.
        /// </summary>
        /// <param name="ms">Memory stream of chunk data.</param>
        protected override void ParseData(MemoryStream ms)
        {
            width = Helper.ConvertEndian(ms.ReadInt32());
            height = Helper.ConvertEndian(ms.ReadInt32());
            bitDepth = Convert.ToByte(ms.ReadByte());
            colorType = Convert.ToByte(ms.ReadByte());
            compressionMethod = Convert.ToByte(ms.ReadByte());
            filterMethod = Convert.ToByte(ms.ReadByte());
            interlaceMethod = Convert.ToByte(ms.ReadByte());
        }
    }
}
