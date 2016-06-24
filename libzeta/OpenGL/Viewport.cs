using System;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace libzeta {

    /// <summary>
    /// Viewport.
    /// </summary>
    public class Viewport : IDisposable {

        /// <summary>
        /// Gets or sets the resolution.
        /// </summary>
        /// <value>The resolution.</value>
        public Resolution Resolution {
            get {
                return new Resolution (Bounds.Width, Bounds.Height);
            }
            set {
                Bounds.Width = value.Width;
                Bounds.Height = value.Height;
            }
        }

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>The position.</value>
        public Vector2 Position {
            get {
                return new Vector2 (Bounds.X, Bounds.Y);
            }
            set {
                Bounds.X = (int)value.X;
                Bounds.Y = (int)value.Y;
            }
        }

        /// <summary>
        /// The bounds.
        /// </summary>
        public Rectangle Bounds;

        /// <summary>
        /// The target model.
        /// </summary>
        readonly Model targetModel;

        /// <summary>
        /// The shader.
        /// </summary>
        readonly internal ShaderProgram shader;

        /// <summary>
        /// The matrix.
        /// </summary>
        readonly Matrix4 projectionMatrix;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:libzeta.Viewport"/> class.
        /// </summary>
        /// <param name="resolution">Resolution.</param>
        /// <param name="position">Position.</param>
        public Viewport (Resolution resolution, Vector2? position = null) {

            // Set the bounds
            Resolution = resolution;
            Position = position ?? Vector2.Zero;

            // Create the position vertices
            var pos = new [] {
                new Vector3 (Bounds.Width, 0, 0),
                new Vector3 (Bounds.Width, Bounds.Height, 0),
                new Vector3 (0, Bounds.Height, 0),
                new Vector3 (0, 0, 0)
            };

            // Create the texture vertices
            var tex = new [] {
                new Vector2 (1f, 0f),
                new Vector2 (1f, 1f),
                new Vector2 (0f, 1f),
                new Vector2 (0f, 0f)
            };

            // Create the vertex and fragment shaders
            var vertexShader = new VertexShader (ShaderCollection.Viewport.VertexShader);
            var fragmentShader = new FragmentShader (ShaderCollection.Viewport.FragmentShader);

            // Create and link the shader program
            shader = new ShaderProgram (vertexShader, fragmentShader);
            shader.Link ();

            // Create the model
            var geometry = new Mesh (BeginMode.Quads);
            geometry.AddBuffer ("v_pos", new GLBuffer<Vector3> (GLBufferSettings.StaticDraw3FloatArray, pos));
            geometry.AddBuffer ("v_tex", new GLBuffer<Vector2> (GLBufferSettings.StaticDraw2FloatArray, tex));
            targetModel = new Model (geometry);

            // Create the projection matrix
            projectionMatrix = Matrix4.CreateOrthographicOffCenter (0, Bounds.Width, 0, Bounds.Height, 0, 16);
        }

        /// <summary>
        /// Draw the specified texture.
        /// </summary>
        /// <param name="texture">Texture.</param>
        public void Draw (Texture2D texture) {

            // Set the viewport up
            GL.Viewport (
                x: Bounds.X,
                y: Bounds.Y,
                width: Bounds.Width,
                height: Bounds.Height
            );

            // Use the shader program
            shader.Use (program => {

                // Set the material texture of the target model
                targetModel.geometry.Material.Texture = texture;

                // Set the position of the target model
                targetModel.Position = new Vector3 (Bounds.X, Bounds.Y, 0);

                // Draw the target model
                targetModel.Draw (program, projectionMatrix);
            });
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose (bool disposing) {
            if (!disposedValue) {
                if (disposing) {
                    shader.Dispose ();
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

