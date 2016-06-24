using System;
namespace libzeta {

    /// <summary>
    /// Rectangle.
    /// </summary>
    public struct Rectangle {

        /// <summary>
        /// The x coordinate.
        /// </summary>
        public int X;

        /// <summary>
        /// The y coordinate.
        /// </summary>
        public int Y;

        /// <summary>
        /// The width.
        /// </summary>
        public int Width;

        /// <summary>
        /// The height.
        /// </summary>
        public int Height;

        /// <summary>
        /// Gets the location.
        /// </summary>
        /// <value>The location.</value>
        public Point2 Location => new Point2 (X, Y);

        /// <summary>
        /// Initializes a new instance of the <see cref="T:libzeta.Rectangle"/> struct.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="w">The width.</param>
        /// <param name="h">The height.</param>
        public Rectangle (int x, int y, int w, int h) : this () {
            X = x;
            Y = y;
            Width = w;
            Height = h;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:libzeta.Rectangle"/> struct.
        /// </summary>
        /// <param name="position">Position.</param>
        /// <param name="size">Size.</param>
        public Rectangle (Point2 position, Point2 size)
            : this (position.X, position.Y, size.X, size.Y) {
        }

        public static implicit operator System.Drawing.Rectangle (Rectangle rect) {
            return new System.Drawing.Rectangle (rect.X, rect.Y, rect.Width, rect.Height);
        }
    }
}

