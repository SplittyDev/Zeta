using System;
using OpenTK;

namespace libzeta {

    /// <summary>
    /// Orthographic camera.
    /// </summary>
    public class OrthographicCamera : Camera {

        /// <summary>
        /// Initializes a new instance of the <see cref="T:libzeta.OrthographicCamera"/> class.
        /// </summary>
        /// <param name="width">Width.</param>
        /// <param name="height">Height.</param>
        /// <param name="nearPlane">Near plane.</param>
        /// <param name="farPlane">Far plane.</param>
        public OrthographicCamera (
            float width, float height, float nearPlane = 0, float farPlane = 16) {

            // Initialize position
            Position = Vector3.Zero;

            // Initialize orientation
            Orientation = Quaternion.Identity;

            // Create the projection matrix
            Projection = CameraHelper.CreateOrthographicProjection (
                width: width,
                height: height,
                nearPlane: nearPlane,
                farPlane: farPlane
            );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:libzeta.OrthographicCamera"/> class.
        /// </summary>
        /// <param name="resolution">Resolution.</param>
        /// <param name="nearPlane">Near plane.</param>
        /// <param name="farPlane">Far plane.</param>
        public OrthographicCamera (
            Resolution resolution, float nearPlane = 0, float farPlane = 16)
            : this (resolution.Width, resolution.Height, nearPlane, farPlane) {
        }
    }
}

