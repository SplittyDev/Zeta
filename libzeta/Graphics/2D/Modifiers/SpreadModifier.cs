using System;
using OpenTK;

namespace libzeta {

    /// <summary>
    /// Spread modifier.
    /// </summary>
    public class SpreadModifier : ParticleModifier {

        readonly bool spreadX;
        readonly bool spreadY;
        readonly float maxSpread;
        readonly float minSpread;
        readonly float [] choices = { -1f, 1f };

        /// <summary>
        /// Initializes a new instance of the <see cref="T:libzeta.SpreadModifier"/> class.
        /// </summary>
        /// <param name="spreadX">Spread x.</param>
        /// <param name="spreadY">Spread y.</param>
        /// <param name="maxSpread">Max spread.</param>
        /// <param name="minSpread">Minimum spread.</param>
        public SpreadModifier (bool spreadX, bool spreadY, float maxSpread = 50, float minSpread = 0f) {
            this.spreadX = spreadX;
            this.spreadY = spreadY;
            this.maxSpread = maxSpread;
            this.minSpread = minSpread;
        }

        public override void ModifyOnSpawn (Game game, GameTime time, Particle particle) {

            // Compute the direction vector
            var direction = new Vector3 (
                x: spreadX ? Randomizer.Choose (choices) : 0,
                y: spreadY ? Randomizer.Choose (choices) : 0,
                z: 0
            );

            // Update the position of the particle
            particle.Position += direction * (minSpread + maxSpread * Randomizer.NextFloat ());
        }
    }
}

