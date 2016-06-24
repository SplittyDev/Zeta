using System;
using OpenTK.Graphics.OpenGL4;

namespace libzeta {

    /// <summary>
    /// Basic shader.
    /// </summary>
    public class BasicShader : Shader, IContentProvider {

        /// <summary>
        /// Initializes a new instance of the <see cref="T:libzeta.BasicShader"/> class.
        /// </summary>
        /// <param name="type">Type.</param>
        /// <param name="sources">Sources.</param>
        public BasicShader (ShaderType type, params string [] sources) : base (type, sources) {
#pragma warning disable RECS0021 // Warns about calls to virtual member functions occuring in the constructor
            Compile ();
#pragma warning restore RECS0021 // Warns about calls to virtual member functions occuring in the constructor
        }

        /// <summary>
        /// Compile the shader.
        /// </summary>
        public override void Compile () {

            // Create the shader
            internalShaderId = GL.CreateShader (shaderType);

            var lengths = new int [shaderSources.Length];
            for (var i = 0; i < lengths.Length; i++)
                lengths [i] = -1;

            // Load the shader sources
            GL.ShaderSource (
                shader: internalShaderId,
                count: shaderSources.Length,
                @string: shaderSources,
                length: ref lengths [0]
            );

            // Compile the shader
            GL.CompileShader (internalShaderId);

            // Get the shader compile status
            int status;
            GL.GetShader (
                shader: internalShaderId,
                pname: ShaderParameter.CompileStatus,
                @params: out status
            );

            // Check if there was an error compiling the shader
            if (status == 0) {

                // Get the error message
                var error = GL.GetShaderInfoLog (internalShaderId);

                // Throw an exception
                throw new Exception ($"Could not compile {shaderType}: {error}");
            }
        }

        /// <summary>
        /// Loads the content.
        /// </summary>
        /// <returns>The content.</returns>
        /// <param name="path">Path.</param>
        public virtual object LoadContent (string path, params object [] args) {
            return FromFile<BasicShader> (path);
        }
    }
}

