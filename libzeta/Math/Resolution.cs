using System;
using OpenTK;

namespace libzeta {

    /// <summary>
    /// Resolution.
    /// </summary>
    public struct Resolution {

        /// <summary>
        /// The width.
        /// </summary>
        public int Width;

        /// <summary>
        /// The height.
        /// </summary>
        public int Height;

        /// <summary>
        /// Gets the aspect ratio.
        /// </summary>
        /// <value>The aspect ratio.</value>
        public float AspectRatio => Width / Height;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:libzeta.Resolution"/> struct.
        /// </summary>
        /// <param name="width">Width.</param>
        /// <param name="height">Height.</param>
        public Resolution (int width, int height) : this () {
            Width = width;
            Height = height;
        }

        public bool InBounds (Vector2 vec) {
            return vec.X > 0 && vec.Y > 0 && vec.X < Width && vec.Y < Height;
        }

        public bool InBounds (Vector3 vec) {
            return InBounds (vec.Xy);
        }
    }
}

