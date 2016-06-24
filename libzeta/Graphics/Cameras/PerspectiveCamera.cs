using System;
using OpenTK;

namespace libzeta {

    /// <summary>
    /// Perspective camera.
    /// </summary>
    public class PerspectiveCamera : Camera {

        /// <summary>
        /// Initializes a new instance of the <see cref="T:libzeta.PerspectiveCamera"/> class.
        /// </summary>
        /// <param name="aspectRatio">Aspect ratio.</param>
        /// <param name="fieldOfView">Field of view.</param>
        /// <param name="nearPlane">Near plane.</param>
        /// <param name="farPlane">Far plane.</param>
        public PerspectiveCamera (
            float aspectRatio, float fieldOfView = 60f, float nearPlane = 0.01f, float farPlane = 100f) {

            // Initialize the position
            Position = Vector3.Zero;

            // Initialize the orientation
            Orientation = Quaternion.Identity;

            // Create the projection matrix
            Projection = CameraHelper.CreatePerspectiveProjection (
                fieldOfView: fieldOfView,
                aspectRatio: aspectRatio,
                nearPlane: nearPlane,
                farPlane: farPlane
            );
        }
    }
}

