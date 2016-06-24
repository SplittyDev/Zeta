using System;
using OpenTK;

namespace libzeta {

    /// <summary>
    /// Projectable interface.
    /// </summary>
    public interface IProjectable {

        /// <summary>
        /// Gets or sets the orientation.
        /// </summary>
        /// <value>The orientation.</value>
        Quaternion Orientation { get; }

        /// <summary>
        /// Gets the view matrix.
        /// </summary>
        /// <value>The view matrix.</value>
        Matrix4 View { get; }

        /// <summary>
        /// Gets or sets the projection matrix.
        /// </summary>
        /// <value>The projection matrix.</value>
        Matrix4 Projection { get; }

        /// <summary>
        /// Gets the view projection matrix.
        /// </summary>
        /// <value>The view projection.</value>
        Matrix4 ViewProjection { get; }

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>The position.</value>
        Vector3 Position { get; }

        /// <summary>
        /// Gets the forward vector.
        /// </summary>
        /// <value>The forward.</value>
        Vector3 Forward { get; }

        /// <summary>
        /// Gets the right vector.
        /// </summary>
        /// <value>The right.</value>
        Vector3 Right { get; }

        /// <summary>
        /// Gets the up vector.
        /// </summary>
        /// <value>Up.</value>
        Vector3 Up { get; }
    }
}

