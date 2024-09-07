using System;
using System.IO;

namespace BeatSaberMarkupLanguage.Animations.APNG.Chunks
{
    /// <summary>
    /// Enumeration of dispose operations.
    /// </summary>
    public enum DisposeOps
    {
        /// <summary>
        /// Does not clear any of the previous drawing.
        /// </summary>
        APNGDisposeOpNone = 0,

        /// <summary>
        /// Clears the background to transparent black before rendering.
        /// </summary>
        APNGDisposeOpBackground = 1,

        /// <summary>
        /// Draws using the previous frame as the base.
        /// </summary>
        APNGDisposeOpPrevious = 2,
    }

    /// <summary>
    /// Enumeration of blend operations.
    /// </summary>
    public enum BlendOps
    {
        /// <summary>
        /// Do not blend use the source data.
        /// </summary>
        APNGBlendOpSource = 0,

        /// <summary>
        /// Perform composite blending.
        /// </summary>
        APNGBlendOpOver = 1,
    }

    /// <summary>
    /// Animated PNG Frame Control chunk.
    /// </summary>
    internal class FcTLChunk : Chunk
    {
        private uint sequenceNumber;
        private uint width;
        private uint height;
        private uint xOffset;
        private uint yOffset;
        private ushort delayNumerator;
        private ushort delayDenominator;
        private DisposeOps disposeOp;
        private BlendOps blendOp;

        /// <summary>
        /// Initializes a new instance of the <see cref="FcTLChunk"/> class.
        /// </summary>
        /// <param name="bytes">Byte Array of chunk data.</param>
        public FcTLChunk(byte[] bytes)
            : base(bytes)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FcTLChunk"/> class.
        /// </summary>
        /// <param name="ms">Memory Stream of chunk data.</param>
        public FcTLChunk(MemoryStream ms)
            : base(ms)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FcTLChunk"/> class.
        /// </summary>
        /// <param name="chunk">Chunk data.</param>
        public FcTLChunk(Chunk chunk)
            : base(chunk)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FcTLChunk"/> class.
        /// </summary>
        internal FcTLChunk()
        {
            Length = 26;
            ChunkType = "fcTL";
            ChunkData = new byte[Length];
        }

        /// <summary>
        /// Gets or sets the sequence number of the animation chunk, starting from 0.
        /// </summary>
        public uint SequenceNumber
        {
            get => sequenceNumber;
            internal set
            {
                sequenceNumber = value;
                ModifyChunkData(0, Helper.ConvertEndian(value));
            }
        }

        /// <summary>
        /// Gets or sets the width of the following frame.
        /// </summary>
        public uint Width
        {
            get => width;
            internal set
            {
                width = value;
                ModifyChunkData(4, Helper.ConvertEndian(value));
            }
        }

        /// <summary>
        /// Gets or sets the height of the following frame.
        /// </summary>
        public uint Height
        {
            get => height;
            internal set
            {
                height = value;
                ModifyChunkData(8, Helper.ConvertEndian(value));
            }
        }

        /// <summary>
        /// Gets or sets the X position at which to render the following frame.
        /// </summary>
        public uint XOffset
        {
            get => xOffset;
            internal set
            {
                xOffset = value;
                ModifyChunkData(12, Helper.ConvertEndian(value));
            }
        }

        /// <summary>
        /// Gets or sets the Y position at which to render the following frame.
        /// </summary>
        public uint YOffset
        {
            get => yOffset;
            internal set
            {
                yOffset = value;
                ModifyChunkData(16, Helper.ConvertEndian(value));
            }
        }

        /// <summary>
        /// Gets or sets the frame delay fraction numerator.
        /// </summary>
        public ushort DelayNumerator
        {
            get => delayNumerator;
            internal set
            {
                delayNumerator = value;
                ModifyChunkData(20, BitConverter.GetBytes(Helper.ConvertEndian(value)));
            }
        }

        /// <summary>
        /// Gets or sets the frame delay fraction denominator.
        /// </summary>
        public ushort DelayDenominator
        {
            get => delayDenominator;
            internal set
            {
                delayDenominator = value;
                ModifyChunkData(22, BitConverter.GetBytes(Helper.ConvertEndian(value)));
            }
        }

        /// <summary>
        /// Gets or sets the type of frame area disposal to be done after rendering this frame.
        /// </summary>
        public DisposeOps DisposeOp
        {
            get => disposeOp;
            internal set
            {
                disposeOp = value;
                ModifyChunkData(24, new[] { (byte)value });
            }
        }

        /// <summary>
        /// Gets or sets the type of frame area rendering for this frame.
        /// </summary>
        public BlendOps BlendOp
        {
            get => blendOp;
            internal set
            {
                blendOp = value;
                ModifyChunkData(25, new[] { (byte)value });
            }
        }

        /// <summary>
        /// Parses the data.
        /// </summary>
        /// <param name="ms">Memory stream of chunk data.</param>
        protected override void ParseData(MemoryStream ms)
        {
            sequenceNumber = Helper.ConvertEndian(ms.ReadUInt32());
            width = Helper.ConvertEndian(ms.ReadUInt32());
            height = Helper.ConvertEndian(ms.ReadUInt32());
            xOffset = Helper.ConvertEndian(ms.ReadUInt32());
            yOffset = Helper.ConvertEndian(ms.ReadUInt32());
            delayNumerator = Helper.ConvertEndian(ms.ReadUInt16());
            delayDenominator = Helper.ConvertEndian(ms.ReadUInt16());
            disposeOp = (DisposeOps)ms.ReadByte();
            blendOp = (BlendOps)ms.ReadByte();
        }
    }
}
