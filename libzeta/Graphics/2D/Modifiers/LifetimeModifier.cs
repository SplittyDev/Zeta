using System;
namespace libzeta {

    /// <summary>
    /// Lifetime modifier.
    /// </summary>
    public class LifetimeModifier : ParticleModifier {

        /// <summary>
        /// The mode.
        /// </summary>
        readonly LifetimeModifierMode mode;

        /// <summary>
        /// The minimum amount.
        /// </summary>
        public float Min;

        /// <summary>
        /// The maximum amount.
        /// </summary>
        public float Max;

        public LifetimeModifier (LifetimeModifierMode mode, float min = 0f, float max = 0.25f) {
            this.mode = mode;
            Min = min;
            Max = max;
        }

        public static LifetimeModifier CreateRandomAdd (float min = 0f, float max = 1f) {
            return new LifetimeModifier (LifetimeModifierMode.RandomAdd, min, max);
        }

        public static LifetimeModifier CreateRandomMultiply (float min = 0.5f, float max = 1.5f) {
            return new LifetimeModifier (LifetimeModifierMode.RandomMultiply, min, max);
        }

        public override void ModifyOnSpawn (Game game, GameTime time, Particle particle) {

            // Switch on the lifetime modifier mode
            switch (mode) {
            case LifetimeModifierMode.RandomAdd:
                particle.Lifetime += Randomizer.Next (Min, Max);
                break;
            case LifetimeModifierMode.RandomMultiply:
                particle.Lifetime *= Randomizer.Next (Min, Max);
                break;
            }
        }
    }
}

