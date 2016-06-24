using System;
using OpenTK;

namespace libzeta {

    /// <summary>
    /// Camera.
    /// </summary>
    public abstract class Camera : GameComponent, IProjectable {

        /// <summary>
        /// Gets the orientation.
        /// </summary>
        /// <value>The orientation.</value>
        public Quaternion Orientation { get; protected set; }

        /// <summary>
        /// Gets the position.
        /// </summary>
        /// <value>The position.</value>
        public Vector3 Position { get; protected set; }

        /// <summary>
        /// Gets the projection matrix.
        /// </summary>
        /// <value>The projection.</value>
        public Matrix4 Projection { get; protected set; }

        /// <summary>
        /// Gets the view matrix.
        /// </summary>
        /// <value>The view.</value>
        public Matrix4 View => CameraHelper.CreateView (Position, Orientation);

        /// <summary>
        /// Gets the view projection matrix.
        /// </summary>
        /// <value>The view projection.</value>
        public Matrix4 ViewProjection => CameraHelper.CreateViewProjection (View, Projection);

        /// <summary>
        /// Gets the forward vector.
        /// </summary>
        /// <value>The forward.</value>
        public Vector3 Forward => CameraHelper.CreateForwardVector (Position, Orientation);

        /// <summary>
        /// Gets the right vector.
        /// </summary>
        /// <value>The right.</value>
        public Vector3 Right => CameraHelper.CreateRightVector (Position, Orientation);

        /// <summary>
        /// Gets up vector.
        /// </summary>
        /// <value>Up.</value>
        public Vector3 Up => CameraHelper.CreateUpVector (Position, Orientation);

        /// <summary>
        /// Looks at the specified target.
        /// </summary>
        /// <param name="target">Target.</param>
        /// <param name="up">Up.</param>
        public void LookAt (Vector3 target, Vector3? up = null) {
            Projection = Matrix4.Mult (Matrix4.LookAt (Position, target, up ?? Up), Projection);
        }

        /// <summary>
        /// Sets the position.
        /// </summary>
        /// <param name="position">Position.</param>
        public void SetPosition (Vector3 position, bool relative = false) {
            Position = (relative ? Position : Vector3.Zero) + position;
        }

        /// <summary>
        /// Sets the rotation in degrees.
        /// </summary>
        /// <param name="axis">Axis.</param>
        /// <param name="degrees">Degrees.</param>
        /// <param name="relative">Relative.</param>
        public void SetRotationDegrees (Vector3 axis, float degrees, bool relative = false) {

            // Compute the rotation amount in radians
            var radians = MathHelper.DegreesToRadians (degrees);

            // Apply the rotation
            SetRotationRadians (axis, radians, relative);
        }

        /// <summary>
        /// Sets the rotation in degrees.
        /// </summary>
        /// <returns>The rotation degrees.</returns>
        /// <param name="pitch">Pitch.</param>
        /// <param name="yaw">Yaw.</param>
        /// <param name="roll">Roll.</param>
        /// <param name="relative">Relative.</param>
        public void SetRotationDegrees (
            float pitch, float yaw, float roll, bool relative) {

            // Compute the rotation amount in radians
            float radPitch = MathHelper.DegreesToRadians (pitch);
            float radYaw = MathHelper.DegreesToRadians (yaw);
            float radRoll = MathHelper.DegreesToRadians (roll);

            // Apply the rotation
            SetRotationRadians (radPitch, radYaw, radRoll, relative);
        }

        /// <summary>
        /// Sets the rotation in radians.
        /// </summary>
        /// <param name="axis">Axis.</param>
        /// <param name="radians">Radians.</param>
        /// <param name="relative">Relative.</param>
        public void SetRotationRadians (Vector3 axis, float radians, bool relative = false) {

            // Calculate the start orientation
            var startOrientation = relative ? Orientation : Quaternion.Identity;

            // Calculate the end orientation
            var endOrientation = Quaternion.FromAxisAngle (axis, radians);

            // Apply the rotation
            Orientation = Quaternion.Multiply (startOrientation, endOrientation);
        }

        /// <summary>
        /// Sets the rotation in radians.
        /// </summary>
        /// <returns>The rotation radians.</returns>
        /// <param name="pitch">Pitch.</param>
        /// <param name="yaw">Yaw.</param>
        /// <param name="roll">Roll.</param>
        /// <param name="relative">Relative.</param>
        public void SetRotationRadians (
            float pitch, float yaw, float roll, bool relative = false) {

            // Calculate the start orientation
            var startOrientation = relative ? Orientation : Quaternion.Identity;

            // Calculate the end orientation
            var endOrientation = Quaternion.FromEulerAngles (pitch, yaw, roll);

            // Apply the rotation
            Orientation = Quaternion.Multiply (startOrientation, endOrientation);
        }
    }
}

