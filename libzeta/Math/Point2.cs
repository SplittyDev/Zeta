using System;
using OpenTK;

namespace libzeta {

    /// <summary>
    /// 2D point.
    /// </summary>
    public struct Point2 {

        /// <summary>
        /// A point with X and Y set to zero.
        /// </summary>
        public static Point2 Zero = new Point2 (0, 0);

        /// <summary>
        /// A point with X and Y set to one.
        /// </summary>
        public static Point2 One = new Point2 (1, 1);

        /// <summary>
        /// The x coordinate.
        /// </summary>
        public int X;

        /// <summary>
        /// The y coordinate.
        /// </summary>
        public int Y;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:libzeta.Point2"/> struct.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        public Point2 (int x, int y) : this () {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:libzeta.Point2"/> struct.
        /// </summary>
        /// <param name="vec">Vec.</param>
        public Point2 (Vector2 vec) : this () {
            X = (int)vec.X;
            Y = (int)vec.Y;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:libzeta.Point2"/> struct.
        /// </summary>
        /// <param name="vec">Vec.</param>
        public Point2 (Vector3 vec) : this (vec.Xy) {
        }

        public static implicit operator System.Drawing.Point (Point2 point) {
            return new System.Drawing.Point (point.X, point.Y);
        }

        public static implicit operator System.Drawing.Size (Point2 point) {
            return new System.Drawing.Size (point.X, point.Y);
        }

        public static implicit operator Point2 (System.Drawing.Point point) {
            return new Point2 (point.X, point.Y);
        }

        public static implicit operator Point2 (System.Drawing.Size size) {
            return new Point2 (size.Width, size.Height);
        }
    }
}

