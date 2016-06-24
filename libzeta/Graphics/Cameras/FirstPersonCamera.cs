using System;
using OpenTK;
using OpenTK.Input;

namespace libzeta {

    /// <summary>
    /// First person camera.
    /// </summary>
    public class FirstPersonCamera : PerspectiveCamera {

        /// <summary>
        /// The game.
        /// </summary>
        readonly Game game;

        /// <summary>
        /// The mouse rotation.
        /// </summary>
        Vector2 mouseRotation;

        /// <summary>
        /// Gets or sets the speed.
        /// </summary>
        /// <value>The speed.</value>
        public float Speed { get; set; }

        /// <summary>
        /// Gets or sets the mouse sensitivity.
        /// </summary>
        /// <value>The mouse sensitivity.</value>
        public Vector2 MouseSensitivity { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:libzeta.FirstPersonCamera"/> class.
        /// </summary>
        /// <param name="aspectRatio">Aspect ratio.</param>
        /// <param name="fieldOfView">Field of view.</param>
        /// <param name="nearPlane">Near plane.</param>
        /// <param name="farPlane">Far plane.</param>
        public FirstPersonCamera (
            Game game, float aspectRatio, float fieldOfView = 60, float nearPlane = 0.01f, float farPlane = 256)
            : base (aspectRatio, fieldOfView, nearPlane, farPlane) {

            // Set the game
            this.game = game;

            // Initialize the speed
            Speed = 10f;

            // Initialize the mouse sensitivity
            MouseSensitivity = new Vector2 (0.1f, 0.1f);
        }

        public override void Update (GameTime time) {
            base.Update (time);

            // Update the mouse rotation values
            var rad90 = 1.5708f;
            var rad360 = 6.28319f;
            var newX = (float)(game.Mouse.DeltaX * MouseSensitivity.X * time.Delta.TotalSeconds);
            var newY = (float)(game.Mouse.DeltaY * MouseSensitivity.Y * time.Delta.TotalSeconds);
            mouseRotation += new Vector2 (newX, newY);

            // Clamp the mouse rotation values
            mouseRotation.Y = MathHelper.Clamp (mouseRotation.Y, -rad90, rad90);
            mouseRotation.X += mouseRotation.X >= rad360 ? -rad360 : 0;
            mouseRotation.X += mouseRotation.X <= -rad360 ? rad360 : 0;

            // Update the orientation
            var quatX = Quaternion.FromAxisAngle (Vector3.UnitX, mouseRotation.Y);
            var quatY = Quaternion.FromAxisAngle (Vector3.UnitY, (MathHelper.Pi + mouseRotation.X));
            Orientation = Quaternion.Multiply (quatX, quatY);

            // Compute the distance
            var distance = Speed * (float)time.Delta.TotalSeconds;

            // Create the movement vector
            var movement = Vector3.Zero;

            // Test if the W key is down
            if (game.Keyboard.IsKeyDown (Key.W))
                movement.Z = -distance;

            // Test if the S key is down
            if (game.Keyboard.IsKeyDown (Key.S))
                movement.Z = distance;

            // Test if the A key is down
            if (game.Keyboard.IsKeyDown (Key.A))
                movement.X = -distance;

            // Test if the D key is down
            if (game.Keyboard.IsKeyDown (Key.D))
                movement.X = distance;

            // Test if the space key is down
            if (game.Keyboard.IsKeyDown (Key.Space))
                movement.Y = distance;

            // Test if the left shift key is down
            if (game.Keyboard.IsKeyDown (Key.LShift)) {
                movement.Y = -distance;
            }

            // Compute the new camera position
            var position = Vector3.Transform (movement, Orientation.Inverted ());
            SetPosition (position, true);
        }
    }
}

