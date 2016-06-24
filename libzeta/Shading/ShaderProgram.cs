using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace libzeta {

    /// <summary>
    /// Shader program.
    /// </summary>
    public sealed class ShaderProgram : IDisposable {

        /// <summary>
        /// The current shader program identifier.
        /// </summary>
        public static int CurrentProgramId;

        /// <summary>
        /// Initializes the <see cref="T:libzeta.ShaderProgram"/> class.
        /// </summary>
        static ShaderProgram () {

            // Initialize the current program id to 0
            CurrentProgramId = 0;
        }

        /// <summary>
        /// The attributes.
        /// </summary>
        readonly Dictionary<string, int> attributes;

        /// <summary>
        /// The uniforms.
        /// </summary>
        readonly Dictionary<string, int> uniforms;

        /// <summary>
        /// The shader objects.
        /// </summary>
        readonly List<Shader> shaderObjects;

        /// <summary>
        /// The program identifier.
        /// </summary>
        int programId;

        /// <summary>
        /// Gets the program identifier.
        /// </summary>
        /// <value>The program identifier.</value>
        public int ProgramId { get { return programId; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:libzeta.ShaderProgram"/> class.
        /// </summary>
        /// <param name="shaders">Shaders.</param>
        public ShaderProgram (params Shader [] shaders) {

            // Initialize uniforms
            uniforms = new Dictionary<string, int> ();

            // Initialize attributes
            attributes = new Dictionary<string, int> ();

            // Create the shader program
            programId = GL.CreateProgram ();

            // Initialize the shaderObjects list with the shaders 
            shaderObjects = new List<Shader> (shaders.Length);

            // Attach all shaders to the program
            foreach (var shader in shaders)
                Attach (shader);
        }

        /// <summary>
        /// Gets or sets the specified uniform.
        /// </summary>
        /// <param name="uniform">Uniform.</param>
        public object this [string uniform] {
            get { return GetUniform (uniform); }
            set { SetUniform (uniform, value); }
        }

        /// <summary>
        /// Gets or sets the specfied uniform by its identifier.
        /// </summary>
        /// <param name="uniformId">Uniform identifier.</param>
        public object this [int uniformId] {
            set { SetUniformValue (uniformId, value); }
        }

        /// <summary>
        /// Use the shader program for the specified action.
        /// </summary>
        /// <param name="act">Act.</param>
        public void Use (Action<ShaderProgram> act) {
            using (UseProgram ())
                act (this);
        }

        /// <summary>
        /// Use the shader program.
        /// </summary>
        /// <returns>The handle of the previous shader program.</returns>
        ShaderProgramHandle UseProgram () {

            // Get the handle of the current shader program
            var handle = new ShaderProgramHandle (CurrentProgramId);

            // Check if the shader program doesn't equal the current one
            if (CurrentProgramId != programId) {

                // Use this shader program
                GL.UseProgram (programId);

                // Make the shader program the current one
                CurrentProgramId = programId;
            }

            // Return the handle of the previous shader program
            return handle;
        }

        /// <summary>
        /// Link the program.
        /// </summary>
        public ShaderProgram Link () {

            // Link the shader program
            GL.LinkProgram (programId);

            // Ge the shader link status
            int status;
            GL.GetProgram (
                program: programId,
                pname: GetProgramParameterName.LinkStatus,
                @params: out status
            );

            // Check if there was an error linking the shader
            if (status == 0) {

                // Get the error message
                var error = GL.GetProgramInfoLog (programId);

                // Throw an exception
                throw new Exception ($"Could not link program: {error}");
            }

            return this;
        }

        /// <summary>
        /// Attach the specified shader.
        /// </summary>
        /// <param name="shader">Shader.</param>
        public void Attach (Shader shader) {

            // Attach the shader to the program
            GL.AttachShader (programId, shader.ShaderId);

            // Add the shader to the shaderObjects list
            shaderObjects.Add (shader);
        }

        public void Detach (Shader shader) {

            // Check if the shader is loaded
            if (shaderObjects.Contains (shader)) {

                // Detach the shader from the program
                GL.DetachShader (programId, shader.ShaderId);

                // Remove the shader from the shaderObjects list
                shaderObjects.Remove (shader);
            }
        }

        /// <summary>
        /// Gets the specified attribute.
        /// </summary>
        /// <param name="attribute">Attribute name.</param>
        public int Attrib (string attribute) {

            // Check if the attribute cache contains the attribute
            // If not, add it to the cache
            if (!attributes.ContainsKey (attribute)) {
                attributes.Add (attribute, GL.GetAttribLocation (programId, attribute));
            }

            // Return the attribute
            return attributes [attribute];
        }

        /// <summary>
        /// Gets the uniform.
        /// </summary>
        /// <returns>The uniform.</returns>
        /// <param name="uniform">Uniform.</param>
        int GetUniform (string uniform) {

            // Check if the uniform exists
            if (!uniforms.ContainsKey (uniform)) {

                // Set the uniform
                uniforms [uniform] = GL.GetUniformLocation (programId, uniform);
            }

            // Return the uniform
            return uniforms [uniform];
        }

        /// <summary>
        /// Sets the uniform.
        /// </summary>
        /// <param name="uniform">Uniform.</param>
        /// <param name="value">Value.</param>
        void SetUniform (string uniform, object value) {

            // Check if another program is loaded
            if (CurrentProgramId != 0 && CurrentProgramId != programId) {

                // Throw an exception
                throw new Exception ($"Cannot set uniform {uniform} on program {programId} because the current program is {CurrentProgramId}.");
            }

            // Set the uniform value
            this [(int)this [uniform]] = value;
        }

        /// <summary>
        /// Sets the uniform value.
        /// </summary>
        /// <param name="uniformId">Uniform identifier.</param>
        /// <param name="value">Value.</param>
        void SetUniformValue (int uniformId, object value) {

            // Check if another program is loaded
            if (CurrentProgramId != 0 && CurrentProgramId != programId) {

                // Throw an exception
                throw new Exception ($"Cannot set uniform {uniformId} on program {programId} because the current program is {CurrentProgramId}.");
            }

            // Set the uniform
            using (UseProgram ()) {
                TypeSwitch.On (value)
                    .Case ((int x) => GL.Uniform1 (uniformId, x))
                    .Case ((uint x) => GL.Uniform1 (uniformId, x))
                    .Case ((float x) => GL.Uniform1 (uniformId, x))
                    .Case ((Vector2 x) => GL.Uniform2 (uniformId, x))
                    .Case ((Vector3 x) => GL.Uniform3 (uniformId, x))
                    .Case ((Vector4 x) => GL.Uniform4 (uniformId, x))
                    .Case ((Quaternion x) => GL.Uniform4 (uniformId, x))
                    .Case ((Color4 x) => GL.Uniform4 (uniformId, x))
                    .Case ((int [] x) => GL.Uniform1 (uniformId, x.Length, x))
                    .Case ((uint [] x) => GL.Uniform1 (uniformId, x.Length, x))
                    .Case ((float [] x) => GL.Uniform1 (uniformId, x.Length, x))
                    .Case ((Matrix4 x) => GL.UniformMatrix4 (uniformId, false, ref x))
                    // .Case ((Color x) => GL.Uniform4 (uniformId, x.R, x.G, x.B, x.A))
                    .Default (x => {
                        throw new Exception ($"GlUniform type {value.GetType ().FullName} is not (yet?) implemented.");
                    });
            }
        }

        /// <summary>
        /// Binds an attribute location.
        /// </summary>
        /// <param name="index">Index.</param>
        /// <param name="name">Name.</param>
        public void BindAttribLocation (int index, string name) {
            GL.BindAttribLocation (programId, index, name);
        }

        /// <summary>
        /// Binds an attribute location.
        /// </summary>
        /// <param name="attrs">Attrs.</param>
        public void BindAttribLocation (params KeyValuePair<int, string> [] attrs) {
            for (var i = 0; i < attrs.Length; i++)
                BindAttribLocation (attrs [i].Key, attrs [i].Value);
        }

        /// <summary>
        /// Binds an attribute location.
        /// </summary>
        /// <param name="attrs">Attrs.</param>
        public void BindAttribLocation (params Tuple<int, string> [] attrs) {
            for (var i = 0; i < attrs.Length; i++)
                BindAttribLocation (attrs [i].Item1, attrs [i].Item2);
        }

        /// <summary>
        /// Binds a fragment data location.
        /// </summary>
        /// <param name="color">Color.</param>
        /// <param name="name">Name.</param>
        public void BindFragDataLocation (int color, string name) {
            GL.BindFragDataLocation (programId, color, name);
        }

        /// <summary>
        /// Binds a fragment data location.
        /// </summary>
        /// <param name="data">Data.</param>
        public void BindFragDataLocation (params KeyValuePair<int, string> [] data) {
            for (var i = 0; i < data.Length; i++)
                BindFragDataLocation (data [i].Key, data [i].Value);
        }

        /// <summary>
        /// Binds a fragment data location.
        /// </summary>
        /// <param name="data">Data.</param>
        public void BindFragDataLocation (params Tuple<int, string> [] data) {
            for (var i = 0; i < data.Length; i++)
                BindFragDataLocation (data [i].Item1, data [i].Item2);
        }

        /// <summary>
        /// Transform feedback varyings.
        /// </summary>
        /// <param name="mode">Mode.</param>
        /// <param name="varyings">Varyings.</param>
        public void TransformFeedbackVaryings (TransformFeedbackMode mode, params string [] varyings) {
            GL.TransformFeedbackVaryings (programId, varyings.Length, varyings, mode);
        }

        /// <summary>
        /// Transform feedback varyings.
        /// </summary>
        /// <param name="data">Data.</param>
        public void TransformFeedbackVaryings (params KeyValuePair<TransformFeedbackMode, string []> [] data) {
            for (var i = 0; i < data.Length; i++)
                TransformFeedbackVaryings (data [i].Key, data [i].Value);
        }

        /// <summary>
        /// Transform feedback varyings.
        /// </summary>
        /// <param name="data">Data.</param>
        public void TransformFeedbackVaryings (params Tuple<TransformFeedbackMode, string []> [] data) {
            for (var i = 0; i < data.Length; i++)
                TransformFeedbackVaryings (data [i].Item1, data [i].Item2);
        }

        #region IDisposable Support
        private bool disposedValue = false;

        void Dispose (bool disposing) {
            if (!disposedValue) {
                if (disposing) {

                    // Detach all shaders from the program
                    while (shaderObjects.Count > 0) {

                        // Get the first shader
                        var shader = shaderObjects [0];

                        // Detach the shader
                        GL.DetachShader (programId, shader.ShaderId);

                        // Remove the shader
                        shaderObjects.Remove (shader);

                        // Dispose the shader
                        shader.Dispose ();
                    }

                    // Delete the program if its id is not -1
                    if (programId != -1)
                        GL.DeleteProgram (programId);

                    // Clear remaining shaders, just in case
                    shaderObjects.Clear ();

                    // Set the program id to -1
                    programId = -1;
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

