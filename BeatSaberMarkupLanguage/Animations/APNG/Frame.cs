using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using BeatSaberMarkupLanguage.Animations.APNG.Chunks;

namespace BeatSaberMarkupLanguage.Animations
{
    /// <summary>
    ///     Describe a single frame.
    /// </summary>
    public class Frame
    {
        /// <summary>
        /// The chunk signature.
        /// </summary>
        public static byte[] Signature = { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };

        /// <summary>
        /// Gets or sets the acTL chunk.
        /// </summary>
        internal IHDRChunk IHDRChunk { get; set; }

        /// <summary>
        /// Gets or sets the fcTL chunk.
        /// </summary>
        internal fcTLChunk fcTLChunk { get; set; }

        /// <summary>
        /// Gets or sets the IEND chunk.
        /// </summary>
        internal IENDChunk IENDChunk { get; set; }

        /// <summary>
        /// Gets or sets the other chunks.
        /// </summary>
        internal List<OtherChunk> OtherChunks { get; set; } = new List<OtherChunk>();

        /// <summary>
        /// Gets or sets the IDAT chunks.
        /// </summary>
        internal List<IDATChunk> IDATChunks { get; set; } = new List<IDATChunk>();

        /// <summary>
        /// Add an Chunk to end end of existing list.
        /// </summary>
        internal void AddOtherChunk(OtherChunk chunk)
        {
            OtherChunks.Add(chunk);
        }

        /// <summary>
        /// Add an IDAT Chunk to end end of existing list.
        /// </summary>
        internal void AddIDATChunk(IDATChunk chunk)
        {
            IDATChunks.Add(chunk);
        }

        /// <summary>
        /// Gets the frame as PNG FileStream.
        /// </summary>
        /// <returns>The PNG file as a <see cref="Stream"/>.</returns>
        public MemoryStream GetStream()
        {
            var ihdrChunk = new IHDRChunk(IHDRChunk);
            if (fcTLChunk != null)
            {
                // Fix frame size with fcTL data.
                ihdrChunk.ModifyChunkData(0, Helper.ConvertEndian(fcTLChunk.Width));
                ihdrChunk.ModifyChunkData(4, Helper.ConvertEndian(fcTLChunk.Height));
            }

            // Write image data
            var ms = new MemoryStream();
            ms.WriteBytes(Signature);
            ms.WriteBytes(ihdrChunk.RawData);
            OtherChunks.ForEach(o => ms.WriteBytes(o.RawData));
            IDATChunks.ForEach(i => ms.WriteBytes(i.RawData));
            ms.WriteBytes(IENDChunk.RawData);

            ms.Position = 0;
            return ms;
        }

        /// <summary>
        /// Converts the Frame to a Bitmap.
        /// </summary>
        /// <returns>The bitmap of the frame.</returns>
        public Bitmap ToBitmap()
        {
            // Create the bitmap
            Bitmap b = (Bitmap)Image.FromStream(GetStream());

            Bitmap final = new Bitmap(IHDRChunk.Width, IHDRChunk.Height);

            Graphics g = Graphics.FromImage(final);
            g.CompositingMode = CompositingMode.SourceOver;
            g.CompositingQuality = CompositingQuality.GammaCorrected;
            g.Clear(Color.FromArgb(0x00000000));
            g.DrawImage(b, fcTLChunk.XOffset, fcTLChunk.YOffset, fcTLChunk.Width, fcTLChunk.Height);

            return final;
        }

        /// <summary>
        /// Gets the frame rate.
        /// </summary>
        /// <value>The frame rate in milliseconds.</value>
        /// <remarks>Should not be less than 10 ms or animation will not occur.</remarks>
        public int FrameRate
        {
            get
            {
                int frameRate = fcTLChunk.DelayNumerator;
                double denominatorOffset = 1000 / fcTLChunk.DelayDenominator;

                // If not millisecond based make it so for easier processing
                if ((int)Math.Round(denominatorOffset) != 1)
                {
                    frameRate = (int)(fcTLChunk.DelayNumerator * denominatorOffset);
                }

                return frameRate;
            }

            internal set
            {
                // Standardize to milliseconds.
                fcTLChunk.DelayNumerator = (ushort)value;
                fcTLChunk.DelayDenominator = 1000;
            }
        }
    }
}
