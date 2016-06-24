using System;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace libzeta {

    /// <summary>
    /// Color modifier.
    /// </summary>
    public class ColorModifier : ParticleModifier {

        readonly int sizeX, sizeY;
        readonly Color4 color;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:libzeta.ColorModifier"/> class.
        /// </summary>
        /// <param name="sizeX">Size x.</param>
        /// <param name="sizeY">Size y.</param>
        /// <param name="color">Color.</param>
        public ColorModifier (int sizeX, int sizeY, Color4 color) {
            this.sizeX = sizeX;
            this.sizeY = sizeY;
            this.color = color;
        }

        public override void ModifyOnSpawn (Game game, GameTime time, Particle particle) {
            
            // Create the texture
            particle.Texture = new Texture2D (TextureConfiguration.Nearest, sizeX, sizeY);

            // Create the pixel array
            var pixels = new Color4 [sizeX * sizeY];
            for (var i = 0; i < sizeX * sizeY; i++) {
                pixels [i] = color;
            }

            // Upload the pixel data to the texture
            particle.Texture.SetData (pixels, null, PixelFormat.Rgba, PixelType.Float);
        }
    }
}

