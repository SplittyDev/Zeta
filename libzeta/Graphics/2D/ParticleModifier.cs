using System;
namespace libzeta {

    /// <summary>
    /// Particle modifier.
    /// </summary>
    public abstract class ParticleModifier {

        /// <summary>
        /// The current emitter.
        /// </summary>
        protected internal ParticleEmitter CurrentEmitter;

        /// <summary>
        /// Modify the specified particle after it spawns.
        /// </summary>
        /// <param name="game">Game.</param>
        /// <param name="time">Time.</param>
        /// <param name="particle">Particle.</param>
        public virtual void ModifyOnSpawn (Game game, GameTime time, Particle particle) {
        }

        /// <summary>
        /// Modify the specified particle when it gets updated.
        /// </summary>
        /// <param name="game">Game.</param>
        /// <param name="time">Time.</param>
        /// <param name="particle">Particle.</param>
        public virtual void ModifyOnUpdate (Game game, GameTime time, Particle particle) {
        }
    }
}

