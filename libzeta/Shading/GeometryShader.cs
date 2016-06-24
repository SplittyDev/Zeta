using System;
using OpenTK.Graphics.OpenGL4;

namespace libzeta {

    /// <summary>
    /// Geometry shader.
    /// </summary>
    public class GeometryShader : BasicShader {

        /// <summary>
        /// Initializes a new instance of the <see cref="T:libzeta.GeometryShader"/> class.
        /// </summary>
        /// <param name="sources">Sources.</param>
        public GeometryShader (params string [] sources) : base (ShaderType.GeometryShader, sources) {
        }

        /// <summary>
        /// Loads the content.
        /// </summary>
        /// <returns>The content.</returns>
        /// <param name="path">Path.</param>
        public override object LoadContent (string path, params object [] args) {
            return FromFile<GeometryShader> (path);
        }
    }
}

