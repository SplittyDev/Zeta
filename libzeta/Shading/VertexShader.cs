using System;
using OpenTK.Graphics.OpenGL4;

namespace libzeta {

    /// <summary>
    /// Vertex shader.
    /// </summary>
    public class VertexShader : BasicShader {

        /// <summary>
        /// Initializes a new instance of the <see cref="T:libzeta.VertexShader"/> class.
        /// </summary>
        /// <param name="sources">Sources.</param>
        public VertexShader (params string [] sources) : base (ShaderType.VertexShader, sources) {
        }

        /// <summary>
        /// Loads the content.
        /// </summary>
        /// <returns>The content.</returns>
        /// <param name="path">Path.</param>
        public override object LoadContent (string path, params object [] args) {
            return FromFile<VertexShader> (path);
        }
    }
}

