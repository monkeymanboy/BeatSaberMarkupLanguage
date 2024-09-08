using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using BeatSaberMarkupLanguage.Animations.APNG.Chunks;

namespace BeatSaberMarkupLanguage.Animations.APNG
{
    /// <summary>
    /// Animated PNG class.
    /// </summary>
    internal class APNG : IAnimatedImage
    {
        private readonly List<Frame> frames = new();
        private MemoryStream ms;
        private Size viewSize;

        /// <summary>
        /// Gets a value indicating whether the file is a simple PNG.
        /// </summary>
        public bool IsSimplePNG { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the default image is part of the animation.
        /// </summary>
        public bool DefaultImageIsAnimated { get; private set; }

        /// <summary>
        /// Gets the base image.
        /// If IsSimplePNG = <see langword="true"/>, returns the only image;
        /// if it is <see langword="false"/>, returns the default image.
        /// </summary>
        public Frame DefaultImage { get; private set; } = new Frame();

        /// <summary>
        /// Gets the frame array.
        /// If IsSimplePNG = <see langword="true"/>, returns empty.
        /// </summary>
        public Frame[] Frames
        {
            get { return IsSimplePNG ? new[] { DefaultImage } : frames.ToArray(); }
        }

        /// <summary>
        /// Gets the frame count.
        /// </summary>
        /// <value>The frame count.</value>
        public int FrameCount => (int)(AcTLChunk?.FrameCount ?? 1);

        /// <summary>
        /// Gets or sets the framerate.
        /// Get returns the framerate of the first frame. Set applies the supplied framerate across all frames.
        /// </summary>
        /// <value>The global frame rate.</value>
        public int FrameRate
        {
            get => GetFrameRate(0);
            set
            {
                for (int i = 0; i < frames.Count; ++i)
                {
                    SetFrameRate(i, value);
                }
            }
        }

        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        /// <value>The size of the displayed animated image.</value>
        public Size ViewSize
        {
            get => this.viewSize;
            set => this.viewSize = value;
        }

        /// <summary>
        /// Gets or sets the actual size.
        /// </summary>
        /// <value>The actual size.</value>
        public Size ActualSize
        {
            get => new(this.IHDRChunk.Width, this.IHDRChunk.Height);
            set
            {
                this.IHDRChunk.Width = value.Width;
                this.IHDRChunk.Height = value.Height;
            }
        }

        /// <summary>
        /// Gets or sets the play count.
        /// </summary>
        /// <value>The play count.</value>
        public int PlayCount
        {
            get => (int)(AcTLChunk?.PlayCount ?? 0);
            set => this.AcTLChunk.PlayCount = (uint)value;
        }

        /// <summary>
        /// Gets the IHDR Chunk.
        /// </summary>
        internal IHDRChunk IHDRChunk { get; private set; }

        /// <summary>
        /// Gets the acTL Chunk.
        /// </summary>
        internal AcTLChunk AcTLChunk { get; private set; }

        /// <summary>
        /// Gets the bitmap at the specified index.
        /// </summary>
        /// <param name="index">The frame index.</param>
        public Bitmap this[int index]
        {
            get
            {
                Bitmap bmp = null;
                if (IsSimplePNG)
                {
                    return new Bitmap(DefaultImage.ToBitmap(), this.viewSize);
                }

                if (index >= 0 && index < frames.Count)
                {
                    // Return bitmap of requested view size
                    bmp = new Bitmap(frames[index].ToBitmap(), this.viewSize);
                }

                return bmp;
            }
        }

        /// <summary>
        /// Creates an Animated PNG from a file.
        /// </summary>
        /// <returns>The file.</returns>
        /// <param name="filename">Filename.</param>
        public static APNG FromFile(string filename)
        {
            APNG apng = new();
            apng.Load(filename);
            return apng;
        }

        /// <summary>
        /// Creates an Animated PNG from a stream.
        /// </summary>
        /// <returns>The file.</returns>
        /// <param name="stream">The stream.</param>
        public static APNG FromStream(MemoryStream stream)
        {
            APNG apng = new();
            apng.Load(stream);
            return apng;
        }

        /// <summary>
        /// Creates an Animated PNG from a Image.
        /// </summary>
        /// <returns>The file.</returns>
        /// <param name="image">Image.</param>
        public static APNG FromImage(Image image)
        {
            APNG apng = new();
            apng.Load(ImageToStream(image));
            return apng;
        }

        /// <summary>
        /// Load the specified png.
        /// </summary>
        /// <param name="filename">The png filename.</param>
        public void Load(string filename)
        {
            Load(File.ReadAllBytes(filename));
        }

        /// <summary>
        /// Load the specified png.
        /// </summary>
        /// <param name="fileBytes">Byte representation of the png file.</param>
        public void Load(byte[] fileBytes)
        {
            MemoryStream stream = new(fileBytes);

            Load(stream);
        }

        /// <summary>
        /// Save the APNG to file.
        /// </summary>
        /// <param name="filename">The filename to output to.</param>
        public void Save(string filename)
        {
            using BinaryWriter writer = new(new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write));
            int frameWriteStartIndex = 0;

            // Write signature
            writer.Write(Frame.Signature);

            // Write header
            writer.Write(this.IHDRChunk.RawData);

            // If acTL exists
            if (AcTLChunk != null)
            {
                // write actl.
                writer.Write(AcTLChunk.RawData);
            }

            // Write all other chunks. (NOTE: These should be consistently the same for all frames)
            foreach (OtherChunk otherChunk in DefaultImage.OtherChunks)
            {
                writer.Write(otherChunk.RawData);
            }

            uint sequenceNumber = 0;

            // If Default Image is not animated
            if (!DefaultImageIsAnimated)
            {
                // write IDAT
                DefaultImage.IDATChunks.ForEach(i => writer.Write(i.RawData));
            }
            else
            {
                frames[0].FcTLChunk.SequenceNumber = sequenceNumber++;

                // Write fcTL
                writer.Write(frames[0].FcTLChunk.RawData);

                // Write IDAT of first frame.
                frames[0].IDATChunks.ForEach(i => writer.Write(i.RawData));

                // Set start frame indes to 1
                frameWriteStartIndex = 1;
            }

            // Foreach frame
            for (int i = frameWriteStartIndex; i < frames.Count; ++i)
            {
                frames[i].FcTLChunk.SequenceNumber = sequenceNumber++;

                // write fcTL
                writer.Write(frames[i].FcTLChunk.RawData);

                // Write fDAT
                for (int j = 0; j < frames[i].IDATChunks.Count; ++j)
                {
                    FdATChunk fdat = FdATChunk.FromIDATChunk(frames[i].IDATChunks[j], sequenceNumber++);

                    writer.Write(fdat.RawData);
                }
            }

            // Write IEnd
            writer.Write(DefaultImage.IENDChunk.RawData);

            writer.Close();
        }

        /// <summary>
        /// Gets the dispose operation for the specified frame.
        /// </summary>
        /// <returns>The dispose operation for the specified frame.</returns>
        /// <param name="index">Index.</param>
        public DisposeOps GetDisposeOperationFor(int index)
        {
            return IsSimplePNG ? DisposeOps.APNGDisposeOpNone : this.frames[index].FcTLChunk.DisposeOp;
        }

        /// <summary>
        /// Gets the blend operation for the specified frame.
        /// </summary>
        /// <returns>The blend operation for the specified frame.</returns>
        /// <param name="index">Index.</param>
        public BlendOps GetBlendOperationFor(int index)
        {
            return IsSimplePNG ? BlendOps.APNGBlendOpSource : this.frames[index].FcTLChunk.BlendOp;
        }

        /// <summary>
        /// Gets the default image.
        /// </summary>
        /// <returns>The default image.</returns>
        public Bitmap GetDefaultImage()
        {
            if (IsSimplePNG)
            {
                return DefaultImage.ToBitmap();
            }

            return this[0];
        }

        /// <summary>
        /// Gets the frame rate for a frame.
        /// </summary>
        /// <returns>The frame rate for a frame.</returns>
        /// <param name="index">The frame index.</param>
        public int GetFrameRate(int index)
        {
            int frameRate = 0;
            if (this.frames != null && this.frames.Count > index)
            {
                frameRate = this.frames[index].FrameRate;
            }

            return frameRate;
        }

        /// <summary>
        /// Sets the frame rate for a frame.
        /// </summary>
        /// <param name="index">The frame index.</param>
        /// <param name="frameRate">The desired frame rate.</param>
        public void SetFrameRate(int index, int frameRate)
        {
            if (this.frames != null && this.frames.Count > index)
            {
                this.frames[index].FrameRate = frameRate;
            }
        }

        /// <summary>
        /// Sets the default image if not a part of the animation.
        /// </summary>
        /// <param name="image">Default image.</param>
        public void SetDefaultImage(Image image)
        {
            this.DefaultImage = FromImage(image).DefaultImage;
            this.DefaultImageIsAnimated = false;
        }

        /// <summary>
        /// Adds an image as the next frame.
        /// </summary>
        /// <param name="image">Png frame.</param>
        public void AddFrame(Image image)
        {
            // TODO: Handle different sizes
            // Temporarily reject improper sizes.
            if (IHDRChunk != null && (image.Width > IHDRChunk.Width || image.Height > IHDRChunk.Height))
            {
                throw new InvalidDataException("Frame must be less than or equal to the size of the other frames.");
            }

            APNG apng = FromImage(image);
            IHDRChunk ??= apng.IHDRChunk;

            // Create acTL Chunk.
            if (AcTLChunk == null)
            {
                AcTLChunk = new AcTLChunk();
                AcTLChunk.PlayCount = 0;
            }

            uint sequenceNumber = (frames.Count == 0) ? 0 : (uint)(frames[^1].FcTLChunk.SequenceNumber + frames[^1].IDATChunks.Count);

            // Create fcTL Chunk
            FcTLChunk fctl = new()
            {
                SequenceNumber = sequenceNumber,
                Width = (uint)image.Width,
                Height = (uint)image.Height,
                XOffset = 0,
                YOffset = 0,
                DelayNumerator = 100,
                DelayDenominator = 1000,
                DisposeOp = DisposeOps.APNGDisposeOpNone,
                BlendOp = BlendOps.APNGBlendOpSource,
            };

            // Set the default image if needed.
            if (DefaultImage.IDATChunks.Count == 0)
            {
                DefaultImage = apng.DefaultImage;
                DefaultImage.FcTLChunk = fctl;
                DefaultImageIsAnimated = true;
            }

            // Add all the frames from the png.
            if (apng.IsSimplePNG)
            {
                Frame frame = apng.DefaultImage;
                frame.FcTLChunk = fctl;

                foreach (OtherChunk chunk in frame.OtherChunks)
                {
                    if (!DefaultImage.OtherChunks.Contains(chunk))
                    {
                        DefaultImage.OtherChunks.Add(chunk);
                    }
                }

                frame.OtherChunks.Clear();
                frames.Add(frame);
            }
            else
            {
                for (int i = 0; i < apng.FrameCount; ++i)
                {
                    Frame frame = apng.Frames[i];
                    frame.FcTLChunk.SequenceNumber = sequenceNumber;
                    foreach (OtherChunk chunk in frame.OtherChunks)
                    {
                        if (!DefaultImage.OtherChunks.Contains(chunk))
                        {
                            DefaultImage.OtherChunks.Add(chunk);
                        }
                    }

                    frame.OtherChunks.Clear();
                    frames.Add(frame);
                }
            }

            List<OtherChunk> otherChunks = DefaultImage.OtherChunks;

            // Now we should apply every chunk in otherChunks to every frame.
            if (DefaultImage != frames[0])
            {
                frames.ForEach(f => otherChunks.ForEach(f.AddOtherChunk));
            }
            else
            {
                for (int i = 1; i < frames.Count; ++i)
                {
                    otherChunks.ForEach(frames[i].AddOtherChunk);
                }
            }

            AcTLChunk.FrameCount = (uint)frames.Count;
        }

        /// <summary>
        /// Removes the specified frame.
        /// </summary>
        /// <param name="index">The frame index.</param>
        public void RemoveFrame(int index)
        {
            frames.RemoveAt(index);
            if (index == 0)
            {
                if (frames.Count == 0)
                {
                    DefaultImage = null;
                    DefaultImageIsAnimated = false;
                }
                else
                {
                    DefaultImage = frames[0];
                }
            }
        }

        /// <summary>
        /// Clears all frames.
        /// </summary>
        public void ClearFrames()
        {
            frames.Clear();
            if (DefaultImageIsAnimated)
            {
                DefaultImage = null;
            }
        }

        /// <summary>
        /// Load the specified stream.
        /// </summary>
        /// <param name="stream">Stream representation of the png file.</param>
        internal void Load(MemoryStream stream)
        {
            ms = stream;

            // check file signature.
            if (!Helper.IsBytesEqual(ms.ReadBytes(Frame.Signature.Length), Frame.Signature))
            {
                throw new APNGException("File signature incorrect.");
            }

            // Read IHDR chunk.
            IHDRChunk = new IHDRChunk(ms);
            if (IHDRChunk.ChunkType != "IHDR")
            {
                throw new APNGException("IHDR chunk must located before any other chunks.");
            }

            viewSize = new Size(IHDRChunk.Width, IHDRChunk.Height);

            // Now let's loop in chunks
            Chunk chunk;
            Frame frame = null;
            List<OtherChunk> otherChunks = new();
            bool isIDATAlreadyParsed = false;
            do
            {
                if (ms.Position == ms.Length)
                {
                    throw new APNGException("IEND chunk expected.");
                }

                chunk = new Chunk(ms);

                switch (chunk.ChunkType)
                {
                    case "IHDR":
                        throw new APNGException("Only single IHDR is allowed.");

                    case "acTL":
                        if (IsSimplePNG)
                        {
                            throw new APNGException("acTL chunk must located before any IDAT and fdAT");
                        }

                        AcTLChunk = new AcTLChunk(chunk);
                        break;

                    case "IDAT":
                        // To be an APNG, acTL must located before any IDAT and fdAT.
                        if (AcTLChunk == null)
                        {
                            IsSimplePNG = true;
                        }

                        // Only default image has IDAT.
                        DefaultImage.IHDRChunk = IHDRChunk;
                        DefaultImage.AddIDATChunk(new IDATChunk(chunk));
                        isIDATAlreadyParsed = true;
                        break;

                    case "fcTL":
                        // Simple PNG should ignore this.
                        if (IsSimplePNG)
                        {
                            continue;
                        }

                        if (frame != null && frame.IDATChunks.Count == 0)
                        {
                            throw new APNGException("One frame must have only one fcTL chunk.");
                        }

                        // IDAT already parsed means this fcTL is used by FRAME IMAGE.
                        if (isIDATAlreadyParsed)
                        {
                            // register current frame object and build a new frame object
                            // for next use
                            if (frame != null)
                            {
                                frames.Add(frame);
                            }

                            frame = new Frame
                            {
                                IHDRChunk = IHDRChunk,
                                FcTLChunk = new FcTLChunk(chunk),
                            };
                        }
                        else
                        {
                            // Otherwise this fcTL is used by the DEFAULT IMAGE.
                            DefaultImage.FcTLChunk = new FcTLChunk(chunk);
                        }

                        break;
                    case "fdAT":
                        // Simple PNG should ignore this.
                        if (IsSimplePNG)
                        {
                            continue;
                        }

                        // fdAT is only used by frame image.
                        if (frame == null || frame.FcTLChunk == null)
                        {
                            throw new APNGException("fcTL chunk expected.");
                        }

                        frame.AddIDATChunk(new FdATChunk(chunk).ToIDATChunk());
                        break;

                    case "IEND":
                        // register last frame object
                        if (frame != null)
                        {
                            frames.Add(frame);
                        }

                        if (DefaultImage.IDATChunks.Count != 0)
                        {
                            DefaultImage.IENDChunk = new IENDChunk(chunk);
                        }

                        foreach (Frame f in frames)
                        {
                            f.IENDChunk = new IENDChunk(chunk);
                        }

                        break;

                    default:
                        otherChunks.Add(new OtherChunk(chunk));
                        break;
                }
            }
            while (chunk.ChunkType != "IEND");

            // We have one more thing to do:
            // If the default image is part of the animation,
            // we should insert it into frames list.
            if (DefaultImage.FcTLChunk != null)
            {
                frames.Insert(0, DefaultImage);
                DefaultImageIsAnimated = true;
            }
            else
            {
                // If it isn't animated it still needs the other chunks.
                otherChunks.ForEach(DefaultImage.AddOtherChunk);
            }

            // Now we should apply every chunk in otherChunks to every frame.
            frames.ForEach(f => otherChunks.ForEach(f.AddOtherChunk));
        }

        /// <summary>
        /// Image to stream.
        /// </summary>
        /// <returns>The to stream.</returns>
        /// <param name="image">Image.</param>
        private static MemoryStream ImageToStream(Image image)
        {
            MemoryStream stream = new();
            image.Save(stream, ImageFormat.Png);
            stream.Position = 0;
            return stream;
        }
    }
}
