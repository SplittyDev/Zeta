using System;
using OpenTK;
using OpenTK.Graphics;

namespace libzeta {

    /// <summary>
    /// Basic light.
    /// </summary>
    public class BasicLight {

        /// <summary>
        /// The color.
        /// </summary>
        public Color4 Color;

        /// <summary>
        /// The intensity.
        /// </summary>
        public float Intensity;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:libzeta.BasicLight"/> class.
        /// </summary>
        /// <param name="color">Color.</param>
        /// <param name="intensity">Intensity.</param>
        public BasicLight (Color4 color, float intensity = 1f) {
            Color = color;
            Intensity = intensity;
        }

        /// <summary>
        /// Converts the color to a 4D vector.
        /// </summary>
        public Vector4 ColorToVector4 () {
            return new Vector4 (Color.R, Color.G, Color.B, Color.A);
        }

        /// <summary>
        /// Converts the color to a 3D vector.
        /// The alpha component is not preserved.
        /// </summary>
        public Vector3 ColorToVector3 () {
            return new Vector3 (Color.R, Color.G, Color.B);
        }
    }
}

