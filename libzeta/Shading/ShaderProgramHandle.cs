using System;
using OpenTK.Graphics.OpenGL4;

namespace libzeta {
    
    /// <summary>
    /// Shader program handle.
    /// </summary>
    public sealed class ShaderProgramHandle : IDisposable {
        
        /// <summary>
        /// The previous shader program.
        /// </summary>
        readonly int previous;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:libzeta.ShaderProgramHandle"/> class.
        /// </summary>
        /// <param name="prev">Previous shader program.</param>
        public ShaderProgramHandle (int prev) {
            previous = prev;
        }

        #region IDisposable Support
        private bool disposedValue = false;

        void Dispose (bool disposing) {
            if (!disposedValue) {
                if (disposing) {

                    // Make sure that the current shader program doesn't equal the previous one
                    if (ShaderProgram.CurrentProgramId != previous) {

                        // Set the current shader program id to the previous one
                        ShaderProgram.CurrentProgramId = previous;

                        // Use the previous shader program
                        GL.UseProgram (previous);
                    }
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

