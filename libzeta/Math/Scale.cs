using System;
namespace libzeta {

    /// <summary>
    /// Scale.
    /// </summary>
    public struct Scale {

        /// <summary>
        /// 0.5x scale.
        /// </summary>
        public static Scale Half = new Scale (0.5f, 0.5f, 1);

        /// <summary>
        /// The original scale.
        /// </summary>
        public static Scale Original = new Scale (1, 1, 1);

        /// <summary>
        /// 2x scale.
        /// </summary>
        public static Scale Double = new Scale (2, 2, 1);

        /// <summary>
        /// 3x scale.
        /// </summary>
        public static Scale Triple = new Scale (3, 3, 1);

        /// <summary>
        /// The scale x.
        /// </summary>
        public float ScaleX;

        /// <summary>
        /// The scale y.
        /// </summary>
        public float ScaleY;

        /// <summary>
        /// The scale z.
        /// </summary>
        public float ScaleZ;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:libzeta.Scale"/> struct.
        /// </summary>
        /// <param name="scaleX">Scale x.</param>
        /// <param name="scaleY">Scale y.</param>
        /// <param name="scaleZ">Scale z.</param>
        public Scale (float scaleX, float scaleY, float scaleZ = 1f) : this () {
            ScaleX = scaleX;
            ScaleY = scaleY;
            ScaleZ = scaleZ;
        }
    }
}

