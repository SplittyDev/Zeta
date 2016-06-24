using System;
using OpenTK;

namespace libzeta {

    /// <summary>
    /// Screen shake.
    /// </summary>
    public class ScreenShake : GameComponent {

        /// <summary>
        /// The currently active camera.
        /// </summary>
        readonly Camera camera;

        /// <summary>
        /// The resolution.
        /// </summary>
        readonly Resolution resolution;

        /// <summary>
        /// The original camera position.
        /// </summary>
        Vector3 cameraPosition;

        /// <summary>
        /// The current position.
        /// </summary>
        Vector3 currentPosition;

        /// <summary>
        /// The amplitude of the shake.
        /// </summary>
        float amplitude;

        /// <summary>
        /// The intensity of the shake.
        /// </summary>
        float intensity;

        /// <summary>
        /// The slope of the shake.
        /// </summary>
        float slope;

        /// <summary>
        /// The elapsed time.
        /// </summary>
        float elapsedTime;

        /// <summary>
        /// Whether the screen is currently shaking.
        /// </summary>
        bool shaking;

        /// <summary>
        /// Whether the screen should shake continuously.
        /// </summary>
        bool continuous;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:libzeta.ScreenShake"/> class.
        /// </summary>
        /// <param name="camera">Camera.</param>
        public ScreenShake (Resolution resolution, Camera camera) {
            this.camera = camera;
            this.resolution = resolution;
        }

        public void Shake (float amplitude, float intensity, float duration, bool continuous = false) {
            if (!shaking) {
                cameraPosition = camera.Position;
            }
            this.amplitude = amplitude;
            this.intensity = intensity;
            this.continuous = continuous;
            slope = amplitude * (1f / duration);
            elapsedTime = 0;
            shaking = true;
        }

        public void ShakeStop () {
            slope = amplitude * 2f;
            continuous = false;
        }

        public override void Update (GameTime time) {
            
            // Test if the screen shake should stop
            if (amplitude <= 0.01f) {
                camera.SetPosition (cameraPosition);
                shaking = false;
                return;
            }

            // Increment the time value
            elapsedTime += (float)time.Delta.TotalSeconds;
            if (continuous) {
                elapsedTime = elapsedTime % 10f;
            }

            // Compute the amplitude
            amplitude -= slope * (float)time.Delta.TotalSeconds;

            // Compute the new camera position
            var position = DefaultShake ();

            // Create the camera position vector
            currentPosition = new Vector3 (position.X, position.Y, cameraPosition.Z);

            // Update the camera position
            camera.SetPosition (currentPosition);
        }

        Vector2 OldShake () {
            var amount = amplitude * (float)Math.Sin (2f * Math.PI * (elapsedTime * intensity));
            return new Vector2 (amount, amount);
        }

        Vector2 DefaultShake () {
            var choices = new [] { -1, 1 };
            var choiceX = Randomizer.Choose (choices);
            var choiceY = Randomizer.Choose (choices);
            var amount = amplitude * (float)Math.Sin (2f * Math.PI * (elapsedTime * intensity));
            var amountX = Math.Max (1, resolution.AspectRatio) * choiceX * amount;
            var amountY = Math.Min (1, resolution.AspectRatio) * choiceY * amount;
            return new Vector2 (amountX, amountY);
        }
    }
}

