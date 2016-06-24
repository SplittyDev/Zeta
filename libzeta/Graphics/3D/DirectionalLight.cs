using System;
using OpenTK;
using OpenTK.Graphics;

namespace libzeta {

    /// <summary>
    /// Directional light.
    /// </summary>
    public class DirectionalLight : BasicLight {

        /// <summary>
        /// The direction.
        /// </summary>
        public Vector3 Direction;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:libzeta.DirectionalLight"/> class.
        /// </summary>
        /// <param name="color">Color.</param>
        /// <param name="direction">Direction.</param>
        /// <param name="intensity">Intensity.</param>
        public DirectionalLight (Color4 color, Vector3 direction, float intensity = 1f)
            : base (color, intensity) {
            Direction = direction;
        }
    }
}

