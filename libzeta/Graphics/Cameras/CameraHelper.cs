using System;
using OpenTK;

namespace libzeta {

    /// <summary>
    /// Camera helper.
    /// </summary>
    static class CameraHelper {

        /// <summary>
        /// Creates an orthographic projection.
        /// </summary>
        /// <returns>The orthographic projection.</returns>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="width">Width.</param>
        /// <param name="height">Height.</param>
        /// <param name="nearPlane">Near plane.</param>
        /// <param name="farPlane">Far plane.</param>
        internal static Matrix4 CreateOrthographicProjection (
            float x, float y, float width, float height, float nearPlane, float farPlane) {

            // Create the orthographic projection
            return Matrix4.CreateOrthographicOffCenter (
                left: x,
                right: width,
                bottom: height,
                top: y,
                zNear: nearPlane,
                zFar: farPlane
            );
        }

        /// <summary>
        /// Creates an orthographic projection.
        /// </summary>
        /// <returns>The orthographic projection.</returns>
        /// <param name="width">Width.</param>
        /// <param name="height">Height.</param>
        /// <param name="nearPlane">Near plane.</param>
        /// <param name="farPlane">Far plane.</param>
        internal static Matrix4 CreateOrthographicProjection (
            float width, float height, float nearPlane, float farPlane)
            => CreateOrthographicProjection (
            x: 0,
            y: 0,
            width: width,
            height: height,
            nearPlane: nearPlane,
            farPlane: farPlane
        );

        /// <summary>
        /// Creates an orthographic projection.
        /// </summary>
        /// <returns>The orthographic projection.</returns>
        /// <param name="resolution">Resolution.</param>
        /// <param name="nearPlane">Near plane.</param>
        /// <param name="farPlane">Far plane.</param>
        internal static Matrix4 CreateOrthographicProjection (
            Resolution resolution, float nearPlane, float farPlane)
            => CreateOrthographicProjection (
            x: 0,
            y: 0,
            width: resolution.Width,
            height: resolution.Height,
            nearPlane: nearPlane,
            farPlane: farPlane
        );

        /// <summary>
        /// Creates a perspective projection.
        /// </summary>
        /// <returns>The perspective projection.</returns>
        /// <param name="fieldOfView">Field of view.</param>
        /// <param name="aspectRatio">Aspect ratio.</param>
        /// <param name="nearPlane">Near plane.</param>
        /// <param name="farPlane">Far plane.</param>
        internal static Matrix4 CreatePerspectiveProjection (
            float fieldOfView, float aspectRatio, float nearPlane, float farPlane) {

            // Create the perspective projection
            return Matrix4.CreatePerspectiveFieldOfView (
                fovy: MathHelper.DegreesToRadians (fieldOfView),
                aspect: aspectRatio,
                zNear: nearPlane,
                zFar: farPlane
            );
        }

        /// <summary>
        /// Creates a view matrix.
        /// </summary>
        /// <returns>The view.</returns>
        /// <param name="position">Position.</param>
        /// <param name="orientation">Orientation.</param>
        internal static Matrix4 CreateView (Vector3 position, Quaternion orientation) {

            // Create the translation matrix
            var matTranslation = Matrix4.CreateTranslation (-position);

            // Create the orientation matrix
            var matOrientation = Matrix4.CreateFromQuaternion (orientation);

            // Create the view matrix
            return Matrix4.Mult (matTranslation, matOrientation);
        }

        /// <summary>
        /// Creates a view projection matrix.
        /// </summary>
        /// <returns>The view projection.</returns>
        /// <param name="view">View.</param>
        /// <param name="projection">Projection.</param>
        internal static Matrix4 CreateViewProjection (Matrix4 view, Matrix4 projection) {

            // Create the view projection matrix by multiplying
            return Matrix4.Mult (view, projection);
        }

        /// <summary>
        /// Creates the forward vector.
        /// </summary>
        /// <returns>The forward vector.</returns>
        /// <param name="position">Position.</param>
        /// <param name="orientation">Orientation.</param>
        internal static Vector3 CreateForwardVector (Vector3 position, Quaternion orientation) {

            // Create the forward vector by transformation around the orientation
            var vecForward = Vector3.Transform (-position, orientation);

            // Return the normalized forward vector
            return vecForward.Normalized ();
        }

        /// <summary>
        /// Creates the right vector.
        /// </summary>
        /// <returns>The right vector.</returns>
        /// <param name="position">Position.</param>
        /// <param name="orientation">Orientation.</param>
        internal static Vector3 CreateRightVector (Vector3 position, Quaternion orientation) {

            // Create the forward vector
            var vecForward = CreateForwardVector (position, orientation);

            // Create the right vector
            return Vector3.Cross (vecForward, Vector3.UnitY);
        }

        /// <summary>
        /// Creates up vector.
        /// </summary>
        /// <returns>The up vector.</returns>
        /// <param name="position">Position.</param>
        /// <param name="orientation">Orientation.</param>
        internal static Vector3 CreateUpVector (Vector3 position, Quaternion orientation) {

            // Create the forward vector
            var vecForward = CreateForwardVector (position, orientation);

            // Create the right vector
            var vecRight = Vector3.Cross (vecForward, Vector3.UnitY);

            // Create the up vector
            return Vector3.Cross (vecRight, vecForward);
        }
    }
}

