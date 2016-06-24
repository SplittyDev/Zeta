using System;
namespace libzeta {

    /// <summary>
    /// Sprite.
    /// </summary>
    public class Sprite {

        /// <summary>
        /// The name of the sprite.
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// The bounds of the sprite.
        /// </summary>
        readonly Rectangle bounds;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:libzeta.Sprite"/> class.
        /// </summary>
        /// <param name="bounds">Bounds.</param>
        public Sprite (Rectangle bounds, string name = null) {
            this.bounds = bounds;
            Name = name ?? string.Empty;
        }

        public static implicit operator Sprite (Rectangle rect) {
            return new Sprite (rect);
        }

        public static implicit operator Rectangle (Sprite sprite) {
            return sprite.bounds;
        }
    }
}

