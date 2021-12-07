using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using BeatSaberMarkupLanguage.Animations.APNG.Chunks;

namespace BeatSaberMarkupLanguage.Animations.APNG
{
    /// <summary>
    /// Animated PNG class.
    /// </summary>
    public class APNG : IAnimatedImage
    {
        private Frame defaultImage = new Frame();
        private List<Frame> frames = new List<Frame>();
        private MemoryStream ms;
        private Size viewSize;

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
            MemoryStream stream = new MemoryStream(fileBytes);

            Load(stream);
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
                throw new Exception("File signature incorrect.");

            // Read IHDR chunk.
            IHDRChunk = new IHDRChunk(ms);
            if (IHDRChunk.ChunkType != "IHDR")
                throw new Exception("IHDR chunk must located before any other chunks.");

            viewSize = new Size(IHDRChunk.Width, IHDRChunk.Height);

            // Now let's loop in chunks
            Chunk chunk;
            Frame frame = null;
            var otherChunks = new List<OtherChunk>();
            bool isIDATAlreadyParsed = false;
            do
            {
                if (ms.Position == ms.Length)
                    throw new Exception("IEND chunk expected.");

                chunk = new Chunk(ms);

                switch (chunk.ChunkType)
                {
                    case "IHDR":
                        throw new Exception("Only single IHDR is allowed.");

                    case "acTL":
                        if (IsSimplePNG)
                            throw new Exception("acTL chunk must located before any IDAT and fdAT");

                        acTLChunk = new acTLChunk(chunk);
                        break;

                    case "IDAT":
                        // To be an APNG, acTL must located before any IDAT and fdAT.
                        if (acTLChunk == null)
                            IsSimplePNG = true;

                        // Only default image has IDAT.
                        defaultImage.IHDRChunk = IHDRChunk;
                        defaultImage.AddIDATChunk(new IDATChunk(chunk));
                        isIDATAlreadyParsed = true;
                        break;

                    case "fcTL":
                        // Simple PNG should ignore this.
                        if (IsSimplePNG)
                            continue;

                        if (frame != null && frame.IDATChunks.Count == 0)
                            throw new Exception("One frame must have only one fcTL chunk.");

                        // IDAT already parsed means this fcTL is used by FRAME IMAGE.
                        if (isIDATAlreadyParsed)
                        {
                            // register current frame object and build a new frame object
                            // for next use
                            if (frame != null)
                                frames.Add(frame);
                            frame = new Frame
                                {
                                    IHDRChunk = IHDRChunk,
                                    fcTLChunk = new fcTLChunk(chunk)
                                };
                        }
                        // Otherwise this fcTL is used by the DEFAULT IMAGE.
                        else
                        {
                            defaultImage.fcTLChunk = new fcTLChunk(chunk);
                        }
                        break;
                    case "fdAT":
                        // Simple PNG should ignore this.
                        if (IsSimplePNG)
                            continue;

                        // fdAT is only used by frame image.
                        if (frame == null || frame.fcTLChunk == null)
                            throw new Exception("fcTL chunk expected.");

                        frame.AddIDATChunk(new fdATChunk(chunk).ToIDATChunk());
                        break;

                    case "IEND":
                        // register last frame object
                        if (frame != null)
                            frames.Add(frame);

                        if (DefaultImage.IDATChunks.Count != 0)
                            DefaultImage.IENDChunk = new IENDChunk(chunk);
                        foreach (Frame f in frames)
                        {
                            f.IENDChunk = new IENDChunk(chunk);
                        }
                        break;

                    default:
                        otherChunks.Add(new OtherChunk(chunk));
                        break;
                }
            } while (chunk.ChunkType != "IEND");

            // We have one more thing to do:
            // If the default image is part of the animation,
            // we should insert it into frames list.
            if (defaultImage.fcTLChunk != null)
            {
                frames.Insert(0, defaultImage);
                DefaultImageIsAnimated = true;
            }
            else //If it isn't animated it still needs the other chunks.
            {
                otherChunks.ForEach(defaultImage.AddOtherChunk);
            }

            // Now we should apply every chunk in otherChunks to every frame.
            frames.ForEach(f => otherChunks.ForEach(f.AddOtherChunk));
        }

        /// <summary>
        /// Save the APNG to file.
        /// </summary>
        /// <param name="filename">The filename to output to.</param>
        public void Save(string filename)
        {
            using (BinaryWriter writer = new BinaryWriter(new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write)))
            {
                int frameWriteStartIndex = 0;

                //Write signature
                writer.Write(Frame.Signature);

                //Write header
                writer.Write(this.IHDRChunk.RawData); 

                //If acTL exists 
                if (acTLChunk != null)
                {
                    //write actl.
                    writer.Write(acTLChunk.RawData);
                }

                //Write all other chunks. (NOTE: These should be consistently the same for all frames)
                foreach(OtherChunk otherChunk in defaultImage.OtherChunks)
                {
                    writer.Write(otherChunk.RawData);
                }

                uint sequenceNumber = 0;
                //If Default Image is not animated
                if (!DefaultImageIsAnimated)
                {
                    //write IDAT
                    defaultImage.IDATChunks.ForEach(i => writer.Write(i.RawData));
                }
                else
                {
                    frames[0].fcTLChunk.SequenceNumber = sequenceNumber++;
                    //Write fcTL
                    writer.Write(frames[0].fcTLChunk.RawData);
                    //Write IDAT of first frame.
                    frames[0].IDATChunks.ForEach(i => writer.Write(i.RawData));
                    //Set start frame indes to 1
                    frameWriteStartIndex = 1;
                }

                //Foreach frame 
                for (int i = frameWriteStartIndex; i < frames.Count; ++i)
                {
                    frames[i].fcTLChunk.SequenceNumber = sequenceNumber++;
                    //write fcTL
                    writer.Write(frames[i].fcTLChunk.RawData);

                    //Write fDAT
                    for(int j = 0; j < frames[i].IDATChunks.Count; ++j)
                    {
                        fdATChunk fdat = fdATChunk.FromIDATChunk(frames[i].IDATChunks[j], sequenceNumber++);

                        writer.Write(fdat.RawData);
                    }
                }

                //Write IEnd
                writer.Write(defaultImage.IENDChunk.RawData);

                writer.Close();
            }
        }

        /// <summary>
        ///     Indicate whether the file is a simple PNG.
        /// </summary>
        public bool IsSimplePNG { get; private set; }

        /// <summary>
        ///     Indicate whether the default image is part of the animation
        /// </summary>
        public bool DefaultImageIsAnimated { get; private set; }

        /// <summary>
        ///     Gets the base image.
        ///     If IsSimplePNG = True, returns the only image;
        ///     if False, returns the default image
        /// </summary>
        public Frame DefaultImage
        {
            get { return defaultImage; }
        }

        /// <summary>
        ///     Gets the frame array.
        ///     If IsSimplePNG = True, returns empty
        /// </summary>
        public Frame[] Frames
        {
            get { return IsSimplePNG ? new Frame[]{defaultImage}:frames.ToArray(); }
        }

        /// <summary>
        /// Gets the dispose operation for the specified frame.
        /// </summary>
        /// <returns>The dispose operation for the specified frame.</returns>
        /// <param name="index">Index.</param>
        public DisposeOps GetDisposeOperationFor(int index)
        {
            return IsSimplePNG ? DisposeOps.APNGDisposeOpNone : this.frames[index].fcTLChunk.DisposeOp;
        }

        /// <summary>
        /// Gets the blend operation for the specified frame.
        /// </summary>
        /// <returns>The blend operation for the specified frame.</returns>
        /// <param name="index">Index.</param>
        public BlendOps GetBlendOperationFor(int index)
        {
            return IsSimplePNG ? BlendOps.APNGBlendOpSource : this.frames[index].fcTLChunk.BlendOp;
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
            else
            {
                return this[0];
            }
        }

        /// <summary>
        /// Gets the bitmap at the specified index.
        /// </summary>
        /// <param name="index">The frame index.</param>
        public Bitmap this[int index]
        {
            get
            {
                Bitmap bmp = null;
                if (IsSimplePNG) return new Bitmap(defaultImage.ToBitmap(), this.viewSize);
                if (index >= 0 && index < frames.Count)
                {
                    //Return bitmap of requested view size
                    bmp = new Bitmap(frames[index].ToBitmap(), this.viewSize);
                }
                return bmp;
            }
        }

        /// <summary>
        /// Gets the frame count.
        /// </summary>
        /// <value>The frame count.</value>
        public int FrameCount
        {
            get
            {
                return (int)(acTLChunk?.FrameCount??1);
            }
        }

        /// <summary>
        /// Gets the frame rate from the first frame as a global frame rate.
        /// Sets the framerate across all frames.
        /// </summary>
        /// <value>The global frame rate.</value>
        public int FrameRate
        {
            get
            {
                return GetFrameRate(0);
            }
            set
            {
                for(int i = 0; i < frames.Count; ++i)
                {
                    SetFrameRate(i, value);
                }
            }
        }

        /// <summary>
        /// Gets the frame rate for a frame.
        /// </summary>
        /// <returns>The frame rate for a frame.</returns>
        /// <param name="index">The frame index.</param>
        public int GetFrameRate(int index)
        {
            int frameRate = 0;
            if(this.frames != null && this.frames.Count > index)
            {
                frameRate = this.frames[index].FrameRate;
            }
            return frameRate;
        }

        /// <summary>
        /// Sets the frame rate for a frame.
        /// </summary>
        /// <param name="index">The frame index</param>
        /// <param name="frameRate">The desired frame rate.</param>
        public void SetFrameRate(int index, int frameRate)
        {
            if(this.frames != null && this.frames.Count > index)
            {
                this.frames[index].FrameRate = frameRate;
            }
        }

        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        /// <value>The size of the displayed animated image.</value>
        public Size ViewSize
        {
            get
            {
                return this.viewSize;
            }
            set
            {
                this.viewSize = value;
            }
        }

        /// <summary>
        /// Gets or sets the actual size.
        /// </summary>
        /// <value>The actual size.</value>
        public Size ActualSize
        {
            get
            {
                return new Size(this.IHDRChunk.Width, this.IHDRChunk.Height);
            }
            set
            {
                this.IHDRChunk.Width = value.Width;
                this.IHDRChunk.Height = value.Height;
            }
        }

        /// <summary>
        /// Gets and sets the play count.
        /// </summary>
        /// <value>The play count.</value>
        public int PlayCount
        {
            get
            {
                return (int)(acTLChunk?.PlayCount??0);
            }
            set
            {
                this.acTLChunk.PlayCount = (uint)value;
            }
        }

        /// <summary>
        ///     Gets the IHDR Chunk
        /// </summary>
        internal IHDRChunk IHDRChunk { get; private set; }

        /// <summary>
        ///     Gets the acTL Chunk
        /// </summary>
        internal acTLChunk acTLChunk { get; private set; }

        /// <summary>
        /// Sets the default image if not a part of the animation.
        /// </summary>
        /// <param name="image">Default image.</param>
        public void SetDefaultImage(Image image)
        {
            this.defaultImage = FromImage(image).DefaultImage;
            this.DefaultImageIsAnimated = false;
        }

        /// <summary>
        /// Adds an image as the next frame.
        /// </summary>
        /// <param name="image">Png frame.</param>
        public void AddFrame(Image image)
        {
            //TODO: Handle different sizes
            //Temporarily reject improper sizes.
            if(IHDRChunk != null && (image.Width > IHDRChunk.Width || image.Height > IHDRChunk.Height))
            {
                throw new InvalidDataException("Frame must be less than or equal to the size of the other frames.");
            }

            APNG apng = APNG.FromImage(image);
            if(IHDRChunk == null)
            {
                IHDRChunk = apng.IHDRChunk;
            }

            //Create acTL Chunk.
            if(acTLChunk == null)
            {
                acTLChunk = new acTLChunk();
                acTLChunk.PlayCount = 0;
            }

            uint sequenceNumber = (frames.Count == 0) ? 0 : (uint)(frames[frames.Count - 1].fcTLChunk.SequenceNumber + frames[frames.Count - 1].IDATChunks.Count);
            //Create fcTL Chunk
            fcTLChunk fctl = new fcTLChunk
                {
                    SequenceNumber = sequenceNumber,
                    Width = (uint)image.Width,
                    Height = (uint)image.Height,
                    XOffset = 0,
                    YOffset = 0,
                    DelayNumerator = 100,
                    DelayDenominator = 1000,
                    DisposeOp = DisposeOps.APNGDisposeOpNone,
                    BlendOp = BlendOps.APNGBlendOpSource
                };

            //Set the default image if needed.
            if(defaultImage.IDATChunks.Count == 0)
            {
                defaultImage = apng.DefaultImage;
                defaultImage.fcTLChunk = fctl;
                DefaultImageIsAnimated = true;
            }

            //Add all the frames from the png.
            if (apng.IsSimplePNG)
            {
                Frame frame = apng.DefaultImage;
                frame.fcTLChunk = fctl;

                foreach(OtherChunk chunk in frame.OtherChunks)
                {
                    if (!defaultImage.OtherChunks.Contains(chunk))
                    {
                        defaultImage.OtherChunks.Add(chunk);
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
                    frame.fcTLChunk.SequenceNumber = sequenceNumber;
                    foreach(OtherChunk chunk in frame.OtherChunks)
                    {
                        if (!defaultImage.OtherChunks.Contains(chunk))
                        {
                            defaultImage.OtherChunks.Add(chunk);
                        }
                    }
                    frame.OtherChunks.Clear();
                    frames.Add(frame);
                }
            }
            List<OtherChunk> otherChunks = defaultImage.OtherChunks;

            // Now we should apply every chunk in otherChunks to every frame.
            if (defaultImage != frames[0])
            {
                frames.ForEach(f => otherChunks.ForEach(f.AddOtherChunk));
            }
            else
            {
                for(int i = 1; i< frames.Count; ++i)
                {
                    otherChunks.ForEach(frames[i].AddOtherChunk);
                }
            }

            acTLChunk.FrameCount = (uint)frames.Count;
        }

        /// <summary>
        /// Removes the specified frame.
        /// </summary>
        /// <param name="index">The frame index.</param>
        public void RemoveFrame(int index)
        {
            frames.RemoveAt(index);
            if(index == 0)
            {
                if (frames.Count == 0)
                {
                    defaultImage = null;
                    DefaultImageIsAnimated = false;
                }
                else
                {
                    defaultImage = frames[0];
                }
            }
        }

        /// <summary>
        /// Clears all frames.
        /// </summary>
        public void ClearFrames()
        {
            frames.Clear();
            if(DefaultImageIsAnimated)
            {
                defaultImage = null;
            }
        }

        /// <summary>
        /// Creates an Animated PNG from a file.
        /// </summary>
        /// <returns>The file.</returns>
        /// <param name="filename">Filename.</param>
        public static APNG FromFile(string filename)
        {
            APNG apng = new APNG();
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
            APNG apng = new APNG();
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
            APNG apng = new APNG();
            apng.Load(APNG.ImageToStream(image));
            return apng;
        }

        /// <summary>
        /// Image to stream.
        /// </summary>
        /// <returns>The to stream.</returns>
        /// <param name="image">Image.</param>
        private static MemoryStream ImageToStream(Image image)
        {
            var stream = new System.IO.MemoryStream();
            image.Save(stream, ImageFormat.Png);
            stream.Position = 0;
            return stream;
        }
    }
}