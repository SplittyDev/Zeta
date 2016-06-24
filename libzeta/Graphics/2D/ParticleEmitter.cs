using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace libzeta {

    /// <summary>
    /// Particle emitter.
    /// </summary>
    public class ParticleEmitter : GameComponent {

        /// <summary>
        /// The game.
        /// </summary>
        readonly Game game;

        /// <summary>
        /// The particles.
        /// </summary>
        readonly List<Particle> particles;

        /// <summary>
        /// The modifiers.
        /// </summary>
        readonly List<ParticleModifier> modifiers;

        /// <summary>
        /// The internal camera.
        /// </summary>
        readonly Camera internalCamera;

        /// <summary>
        /// The duration of the process/burst in seconds.
        /// </summary>
        float duration;

        /// <summary>
        /// The total duration.
        /// </summary>
        float totalDuration;

        /// <summary>
        /// The accumulated time.
        /// </summary>
        float accumulator;

        /// <summary>
        /// The hard time limit.
        /// </summary>
        float hardTimeLimit;

        /// <summary>
        /// Whether the emitter is currently running.
        /// </summary>
        bool running;

        /// <summary>
        /// The position.
        /// </summary>
        public Vector3 Position;

        /// <summary>
        /// The amount of particles to emit per second.
        /// </summary>
        public float Amount;

        /// <summary>
        /// The initial speed of particles in px/second.
        /// </summary>
        public float Speed;

        /// <summary>
        /// The initial lifetime of particles.
        /// </summary>
        public float Lifetime;

        /// <summary>
        /// Gets the duration of the process/burst in seconds.
        /// </summary>
        /// <value>The duration of the process/burst in seconds.</value>
        public float Duration => duration;

        /// <summary>
        /// Gets the particle count.
        /// </summary>
        /// <value>The particle count.</value>
        public int ParticleCount => particles.Count;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:libzeta.ParticleEmitter"/> class.
        /// </summary>
        ParticleEmitter () {
            particles = new List<Particle> ();
            modifiers = new List<ParticleModifier> ();
            Speed = 1000f; // 100 pixels per second
            Amount = 50f; // 25 particles per second
            Lifetime = 1f; // 10 seconds
            hardTimeLimit = -1f; // no hard time limit
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:libzeta.ParticleEmitter"/> class.
        /// </summary>
        /// <param name="game">Game.</param>
        /// <param name="position">Position.</param>
        public ParticleEmitter (Game game, Vector3? position = null) : this () {
            this.game = game;
            Position = position ?? Vector3.Zero;
            internalCamera = new OrthographicCamera (game.Resolution);
        }

        /// <summary>
        /// Set the initial speed of particles.
        /// </summary>
        /// <param name="speed">Speed in px/second.</param>
        public ParticleEmitter SetSpeed (float speed) {
            Speed = speed;
            return this;
        }

        /// <summary>
        /// Set the initial lifetime of particles.
        /// </summary>
        /// <param name="lifetime">Lifetime in seconds.</param>
        public ParticleEmitter SetLifetime (float lifetime) {
            Lifetime = lifetime;
            return this;
        }

        /// <summary>
        /// Set the amount of particles to be emitted.
        /// </summary>
        /// <returns>The amount.</returns>
        /// <param name="amount">Amount in particles/second.</param>
        public ParticleEmitter SetAmount (float amount) {
            Amount = amount;
            return this;
        }

        /// <summary>
        /// Start emitting particles.
        /// </summary>
        public ParticleEmitter Start () {
            duration = 0f;
            totalDuration = 0f;
            hardTimeLimit = -1f;
            running = true;
            return this;
        }

        /// <summary>
        /// Stop emitting particles.
        /// </summary>
        public ParticleEmitter Stop () {
            running = false;
            return this;
        }

        /// <summary>
        /// Stops emitting particles after <paramref name="n"/> seconds.
        /// </summary>
        /// <param name="n">The seconds that have to pass for the emitter to stop automatically.</param>
        public ParticleEmitter StopAfter (float n) {
            hardTimeLimit = n;
            return this;
        }

        /// <summary>
        /// Adds the specified modifier.
        /// </summary>
        /// <returns>The modifier.</returns>
        /// <param name="modifier">Modifier.</param>
        public ParticleEmitter AddModifier (ParticleModifier modifier) {
            modifiers.Add (modifier);
            return this;
        }

        /// <summary>
        /// Removes the specified modifier.
        /// </summary>
        /// <returns>The modifier.</returns>
        /// <param name="modifier">Modifier.</param>
        public void RemoveModifier (ParticleModifier modifier) {
            modifiers.Remove (modifier);
        }

        /// <summary>
        /// Update the particles.
        /// </summary>
        /// <param name="time">Time.</param>
        public override void Update (GameTime time) {

            // Compute the total elapsed seconds since the last update
            var totalSeconds = (float)time.Delta.TotalSeconds;

            // Remove all particles whose age has exceeded their lifetime
            particles.RemoveAll (particle => particle.Age >= particle.Lifetime);

            // Return if the emitter is not running
            if (!running) {
                return;
            }

            // Iterate over the active particles
            foreach (var particle in particles) {

                // Increment the age of the particle
                particle.Age += totalSeconds;

                // Iterate over the modifiers
                foreach (var modifier in modifiers) {

                    // Modify the current particle
                    modifier.CurrentEmitter = this;
                    modifier.ModifyOnUpdate (game, time, particle);
                }
            }

            // Compute the number of particles to emit
            accumulator += Amount * totalSeconds;

            // Test if the accumulator is greater than or equal to one
            if (accumulator >= 1f) {

                // Loop until the accumulator is less than one
                while (accumulator >= 1f) {

                    // Create a new particle
                    var particle = new Particle {
                        Speed = Speed,
                        Lifetime = Lifetime,
                        Position = Position
                    };

                    // Iterate over the modifiers
                    foreach (var modifier in modifiers) {

                        // Modify the particle
                        modifier.CurrentEmitter = this;
                        modifier.ModifyOnSpawn (game, time, particle);
                    }

                    // Add the particle to the list of active particles
                    particles.Add (particle);

                    // Subtract one from the accumulator
                    accumulator -= 1f;
                }

                // Sort the particles by texture id
                if (time.Total.Milliseconds % 5 == 0) {
                    particles.Sort ((a, b) => b.Texture.TextureId - a.Texture.TextureId);
                }
            }

            // Increment the duration by the elapsed seconds
            duration += totalSeconds;
            totalDuration += totalSeconds;

            // Stop emitting new particles if the duration
            // exceeds the hard time limit.
            if (hardTimeLimit >= 0f && totalDuration >= hardTimeLimit) {
                Stop ();
            }
        }

        /// <summary>
        /// Draw the particles.
        /// </summary>
        public override void Draw () {

            // Get a value indicating whether the sprite batch is currently active
            var spriteBatchActive = game.SpriteBatch.Active;

            // Begin batching sprites if the sprite batch is not already active
            if (!spriteBatchActive) {
                game.SpriteBatch.Begin ();
            }

            // Iterate over the active particles
            foreach (var particle in particles) {

                // Continue if the age of the particle exceeds its lifetime
                if (particle.Age >= particle.Lifetime)
                    continue;

                // Continue if the particle is not in the bounds of the window
                if (!game.Resolution.InBounds (particle.Position))
                    continue;

                /*
                // Create the position vertices
                var pos = new [] {
                    new Vector3 (particle.Texture.Width, 0, 0),
                    new Vector3 (particle.Texture.Width, particle.Texture.Height, 0),
                    new Vector3 (0, particle.Texture.Height, 0),
                    new Vector3 (0, 0, 0)
                };

                // Create the texture vertices
                var tex = new [] {
                    new Vector2 (1f, 0f),
                    new Vector2 (1f, 1f),
                    new Vector2 (0f, 1f),
                    new Vector2 (0f, 0f)
                };

                var mesh = new Mesh (BeginMode.Quads);
                mesh.AddBuffer ("v_pos", new GLBuffer<Vector3> (GLBufferSettings.StaticDraw3FloatArray, pos));
                mesh.AddBuffer ("v_tex", new GLBuffer<Vector2> (GLBufferSettings.StaticDraw2FloatArray, tex));
                mesh.Material = new Material (Color4.White, particle.Texture);
                var model = new Model (mesh);
                game.Viewport.shader.Use (program => {
                    model.Draw (program, internalCamera);
                });
                */

                // Draw the particle
                game.SpriteBatch.Draw (
                    texture: particle.Texture,
                    destLocation: new Point2 (particle.Position),
                    tint: particle.Tint
                );
            }

            // End batching sprites if the sprite batch was inactive before
            if (!spriteBatchActive) {
                game.SpriteBatch.End ();
            }
        }
    }
}

