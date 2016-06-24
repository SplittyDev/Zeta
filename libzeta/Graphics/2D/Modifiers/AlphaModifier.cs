using System;
using OpenTK.Graphics;

namespace libzeta {

    /// <summary>
    /// Alpha modifier.
    /// </summary>
    public class AlphaModifier : ParticleModifier {

        /// <summary>
        /// The slope.
        /// </summary>
        public float Slope;

        /// <summary>
        /// The initial value.
        /// </summary>
        public float InitialValue;

        bool randomizeInitialValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:libzeta.AlphaModifier"/> class.
        /// </summary>
        /// <param name="slope">Slope.</param>
        /// <param name="initval">Initial value.</param>
        public AlphaModifier (float slope = 0.1f, bool randomizeInitialValue = false, float initval = 1f) {
            this.randomizeInitialValue = randomizeInitialValue;
            Slope = slope;
            InitialValue = initval;
        }

        public override void ModifyOnSpawn (Game game, GameTime time, Particle particle) {

            // Randomize the initial value if requested
            if (randomizeInitialValue) {
                InitialValue = Randomizer.NextFloat ();
            }

            // Set the initial tint
            particle.Tint = new Color4 (
                r: particle.Tint.R,
                g: particle.Tint.G,
                b: particle.Tint.B,
                a: MathF.Clamp (InitialValue, 0f, 1f)
            );
        }

        public override void ModifyOnUpdate (Game game, GameTime time, Particle particle) {

            // Return if the slope is zero or less
            if (Math.Abs (Slope) < float.Epsilon) {
                return;
            }

            // Compute the alpha amount to subtract from the tint
            var amount = Slope * (float)time.Delta.TotalSeconds;

            // Set the new tint
            particle.Tint = new Color4 (
                r: particle.Tint.R,
                g: particle.Tint.G,
                b: particle.Tint.B,
                a: MathF.Clamp (particle.Tint.A - amount, 0f, 1f)
            );

            // Kill the particle if its alpha value is zero
            if (Math.Abs (particle.Tint.A) < float.Epsilon) {
                particle.Age = particle.Lifetime;
            }
        }
    }
}

