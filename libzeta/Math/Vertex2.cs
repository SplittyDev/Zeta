using System;
using OpenTK;
using OpenTK.Graphics;

namespace libzeta {

    /// <summary>
    /// 2D vertex.
    /// </summary>
    public struct Vertex2 {

        /// <summary>
        /// The position.
        /// </summary>
        public Vector3 Position;

        /// <summary>
        /// The texture coordinate.
        /// </summary>
        public Vector2 TextureCoordinate;

        /// <summary>
        /// The color.
        /// </summary>
        public Color4 Color;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:libzeta.Vertex2D"/> struct.
        /// </summary>
        /// <param name="pos">Position.</param>
        /// <param name="texcoord">Texcoord.</param>
        /// <param name="color">Color.</param>
        public Vertex2 (Vector3 pos, Vector2 texcoord, Color4 color) : this () {
            Position = pos;
            TextureCoordinate = texcoord;
            Color = color;
        }
    }
}

