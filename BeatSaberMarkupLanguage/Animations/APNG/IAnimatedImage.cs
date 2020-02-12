using BeatSaberMarkupLanguage.Animations.Chunks;
using System;
using System.Drawing;

namespace BeatSaberMarkupLanguage.Animations
{
    /// <summary>
    /// Animated Image interface.
    /// </summary>
    public interface IAnimatedImage
    {
        /// <summary>
        /// Gets the bitmap at the specified index.
        /// </summary>
        /// <param name="index">Index of the animation frame.</param>
        Bitmap this[int index]
        {
            get;
        }

        /// <summary>
        /// Gets or sets the frame rate.
        /// </summary>
        /// <value>The frame rate.</value>
        int FrameRate
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the frame count.
        /// </summary>
        /// <value>The frame count.</value>
        int FrameCount
        {
            get;
        }

        /// <summary>
        /// Gets and sets the play count.
        /// </summary>
        /// <value>The play count.</value>
        int PlayCount
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the frame rate of a specific frame
        /// </summary>
        /// <returns>The frame rate of a specific frame</returns>
        /// <param name="index">The frame index.</param>
        int GetFrameRate(int index);

        /// <summary>
        /// Sets the frame rate of a specific frame
        /// </summary>
        /// <param name="index">The frame index.</param>
        /// <param name="frameRate">Frame rate in milliseconds.</param>
        void SetFrameRate(int index, int frameRate);

        /// <summary>
        /// Gets the dispose operation for the specfied frame.
        /// </summary>
        /// <returns>The dispose operation for the specified frame.</returns>
        /// <param name="index">Index of the animation frame.</param>
        DisposeOps GetDisposeOperationFor(int index);

        /// <summary>
        /// Gets the blend operation for the specfied frame.
        /// </summary>
        /// <returns>The blend operation for the specified frame.</returns>
        /// <param name="index">Index of the animation frame.</param>
        BlendOps GetBlendOperationFor(int index);

        /// <summary>
        /// Gets the default image.
        /// </summary>
        /// <returns>The default image.</returns>
        Bitmap GetDefaultImage();

        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        /// <value>The size of the displayed animated image.</value>
        Size ViewSize
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the actual size.
        /// </summary>
        /// <value>The actual size.</value>
        Size ActualSize
        {
            get;
            set;
        }
    }
}

