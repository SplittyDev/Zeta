using System;
using OpenTK;
using OpenTK.Graphics;

namespace libzeta {

    /// <summary>
    /// Particle.
    /// </summary>
    public class Particle {
        
        /// <summary>
        /// The speed in px/second.
        /// </summary>
        public float Speed;

        /// <summary>
        /// The lifetime in seconds.
        /// </summary>
        public float Lifetime;

        /// <summary>
        /// The age in seconds.
        /// </summary>
        public float Age;

        /// <summary>
        /// The position.
        /// </summary>
        public Vector3 Position;

        /// <summary>
        /// The texture.
        /// </summary>
        public Texture2D Texture;

        /// <summary>
        /// The tint.
        /// </summary>
        public Color4 Tint;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:libzeta.Particle"/> class.
        /// </summary>
        public Particle () {
            Tint = Color4.White;
            Texture = Texture2D.Transparent;
        }
    }
}

