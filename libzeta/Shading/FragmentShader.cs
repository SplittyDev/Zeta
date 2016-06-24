using System;
using OpenTK.Graphics.OpenGL4;

namespace libzeta {

    /// <summary>
    /// Fragment shader.
    /// </summary>
    public class FragmentShader : BasicShader {

        /// <summary>
        /// Initializes a new instance of the <see cref="T:libzeta.FragmentShader"/> class.
        /// </summary>
        /// <param name="sources">Sources.</param>
        public FragmentShader (params string [] sources) : base (ShaderType.FragmentShader, sources) {
        }

        /// <summary>
        /// Loads the content.
        /// </summary>
        /// <returns>The content.</returns>
        /// <param name="path">Path.</param>
        public override object LoadContent (string path, params object [] args) {
            return FromFile<FragmentShader> (path);
        }
    }
}

