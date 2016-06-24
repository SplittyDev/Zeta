using System;
using OpenTK;

namespace libzeta {

    /// <summary>
    /// Direction modifier.
    /// </summary>
    public class DirectionModifier : ParticleModifier {

        /// <summary>
        /// The direction.
        /// </summary>
        Vector3 direction;

        /// <summary>
        /// Gets or sets the direction.
        /// </summary>
        /// <value>The direction.</value>
        public Vector3 Direction {
            get {
                return direction;
            } set {
                direction = MathF.Clamp (value, -1f, 1f);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:libzeta.DirectionModifier"/> class.
        /// </summary>
        /// <param name="direction">Direction.</param>
        public DirectionModifier (Vector3 direction) {
            Direction = direction;
        }

        public override void ModifyOnUpdate (Game game, GameTime time, Particle particle) {

            // Compute the distance
            var distance = (float)time.Delta.TotalSeconds * particle.Speed;

            // Update the position of the particle
            particle.Position += direction * distance;
        }
    }
}

