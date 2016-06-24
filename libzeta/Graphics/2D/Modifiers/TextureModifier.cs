using System;

namespace libzeta {

    /// <summary>
    /// Texture modifier.
    /// </summary>
    public class TextureModifier : ParticleModifier {

        /// <summary>
        /// The textures.
        /// </summary>
        readonly Texture2D [] textures;
        readonly bool alwaysRandomize;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:libzeta.TextureModifier"/> class.
        /// </summary>
        /// <param name="textures">Textures.</param>
        public TextureModifier (bool alwaysRandomize = false, params Texture2D [] textures) {
            this.alwaysRandomize = alwaysRandomize;
            this.textures = textures;
        }

        public override void ModifyOnSpawn (Game game, GameTime time, Particle particle) {

            // Choose the current texture from the available textures
            particle.Texture = Randomizer.Choose (textures ?? new [] { particle.Texture });
        }

        public override void ModifyOnUpdate (Game game, GameTime time, Particle particle) {

            // Return if the texture should be randomized on update
            if (!alwaysRandomize) {
                return;
            }

            // Choose the current texture from the available textures
            particle.Texture = Randomizer.Choose (textures ?? new [] { particle.Texture });
        }
    }
}

