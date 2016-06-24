using System;
using OpenTK.Graphics.OpenGL4;

namespace libzeta {

    /// <summary>
    /// Compute shader.
    /// </summary>
    public class ComputeShader : BasicShader {

        /// <summary>
        /// Initializes a new instance of the <see cref="T:libzeta.ComputeShader"/> class.
        /// </summary>
        /// <param name="sources">Sources.</param>
        public ComputeShader (params string [] sources) : base (ShaderType.ComputeShader, sources) {
        }

        /// <summary>
        /// Loads the content.
        /// </summary>
        /// <returns>The content.</returns>
        /// <param name="path">Path.</param>
        public override object LoadContent (string path, params object [] args) {
            return FromFile<ComputeShader> (path);
        }
    }
}

