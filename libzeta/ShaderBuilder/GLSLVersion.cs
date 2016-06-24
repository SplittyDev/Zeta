using System;
namespace libzeta {

    /// <summary>
    /// GLSL version.
    /// </summary>
    [AttributeUsage (AttributeTargets.Class)]
    public class GLSLVersion : Attribute {

        /// <summary>
        /// The version.
        /// </summary>
        public readonly int Version;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:libzeta.GLSLVersion"/> class.
        /// </summary>
        /// <param name="version">Version.</param>
        public GLSLVersion (int version) {
            Version = version;
        }
    }
}

