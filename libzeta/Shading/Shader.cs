using System;
using System.IO;
using System.Text;
using OpenTK.Graphics.OpenGL4;

namespace libzeta {

    /// <summary>
    /// Basic shader.
    /// </summary>
    public abstract class Shader : IDisposable {

        /// <summary>
        /// The type of the shader.
        /// </summary>
        protected ShaderType shaderType;

        /// <summary>
        /// The sources.
        /// </summary>
        protected string [] shaderSources;

        /// <summary>
        /// The shader identifier.
        /// </summary>
        protected int internalShaderId;

        /// <summary>
        /// Gets the shader identifier.
        /// </summary>
        /// <value>The shader identifier.</value>
        public int ShaderId { get { return internalShaderId; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:libzeta.Shader"/> class.
        /// </summary>
        /// <param name="type">Type.</param>
        /// <param name="sources">Sources.</param>
        protected Shader (ShaderType type, params string [] sources) {
            shaderType = type;
            shaderSources = sources;
        }

        /// <summary>
        /// Compile the shader.
        /// </summary>
        public abstract void Compile ();

        /// <summary>
        /// Create a shader with the specified type.
        /// </summary>
        /// <param name="sources">Sources.</param>
        public static Shader Create<Shader> (params string [] sources) where Shader : BasicShader {
            // Return basic shader
            return (Shader)Activator.CreateInstance (typeof (Shader), sources);
        }

        /// <summary>
        /// Creates a shader from the specified file
        /// </summary>
        /// <returns>The shader.</returns>
        /// <param name="path">Path.</param>
        public static Shader FromFile<Shader> (string path) where Shader : BasicShader {

            // Get the full path
            var fullpath = Path.GetFullPath (path);

            // Throw if the file doesn't exist
            if (!File.Exists (fullpath)) {
                throw new FileNotFoundException ($"Unable to load {typeof (Shader).Name} from file. Reason: File not found (Path='{path}')");
            }

            // Read the source code from the file
            var source = File.ReadAllText (fullpath, Encoding.ASCII);

            // Create and return the shader
            return Create<Shader> (source);
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose (bool disposing) {
            if (!disposedValue) {
                if (disposing) {

                    // Delete the shader if its id is not -1
                    if (internalShaderId != -1)
                        GL.DeleteShader (internalShaderId);

                    // Set the shader id to -1
                    internalShaderId = -1;
                }
                disposedValue = true;
            }
        }

        public void Dispose () {
            Dispose (true);
        }
        #endregion
    }
}

