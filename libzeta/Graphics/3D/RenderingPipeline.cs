using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using static libzeta.ShaderCollection.Lighting;

namespace libzeta {

    /// <summary>
    /// Rendering pipeline.
    /// </summary>
    public class RenderingPipeline : IDisposable {

        /// <summary>
        /// The game.
        /// </summary>
        readonly Game game;

        /// <summary>
        /// The frame buffer.
        /// </summary>
        readonly FrameBuffer frameBuffer;

        /// <summary>
        /// The ambient shader.
        /// </summary>
        readonly ShaderProgram ambientShader;

        /// <summary>
        /// The directional shader.
        /// </summary>
        readonly ShaderProgram directionalShader;

        /// <summary>
        /// The directional lights.
        /// </summary>
        readonly List<DirectionalLight> directionalLights;

        /// <summary>
        /// The color of the ambient.
        /// </summary>
        public Color4 AmbientColor;

        /// <summary>
        /// Gets the ambient color vector.
        /// </summary>
        /// <value>The ambient color vector.</value>
        Vector3 ambientColorVector => new Vector3 (AmbientColor.R, AmbientColor.G, AmbientColor.B);

        /// <summary>
        /// Initializes a new instance of the <see cref="T:libzeta.RenderingPipeline"/> class.
        /// </summary>
        /// <param name="game">Game.</param>
        public RenderingPipeline (Game game) {

            // Set the game
            this.game = game;

            // Initialize the list of directional lights
            directionalLights = new List<DirectionalLight> ();

            // Create the frame buffer
            frameBuffer = new FrameBuffer (
                width: game.Resolution.Width,
                height: game.Resolution.Height,
                target: FramebufferTarget.Framebuffer
            );

            // Create the ambient shader
            var ambientVertShader = new VertexShader (AmbientVertexShader);
            var ambientFragShader = new FragmentShader (AmbientFragmentShader);
            ambientShader = new ShaderProgram (ambientVertShader, ambientFragShader);
            ambientShader.Link ();

            // Create the directional shader
            var directionalVertShader = new VertexShader (DirectionalVertexShader);
            var directionalFragShader = new FragmentShader (DirectionalFragmentShader);
            directionalShader = new ShaderProgram (directionalVertShader, directionalFragShader);
            directionalShader.Link ();

            // Initialize the ambient color
            AmbientColor = new Color4 (0.25f, 0.25f, 0.25f, 1f);
        }

        /// <summary>
        /// Adds a directional light.
        /// </summary>
        /// <returns>The directional light.</returns>
        /// <param name="light">Light.</param>
        public void AddDirectionalLight (DirectionalLight light) {
            directionalLights.Add (light);
        }

        public void Begin () {
            frameBuffer.Bind ();
            frameBuffer.Clear ();
        }

        public void End (Viewport viewport = null) {
            frameBuffer.Unbind ();
            Display ();
        }

        /// <summary>
        /// Invokes the specified draw callback on the render pipeline.
        /// </summary>
        /// <param name="camera">Camera.</param>
        /// <param name="drawCallback">Draw callback.</param>
        public void Draw (Camera camera, Action<ShaderProgram> drawCallback) {

            // Clear the color and depth buffer bits
            GL.Clear (ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // Set the ambient color
            ambientShader ["ambient_color"] = ambientColorVector;

            // Draw the scene using the directional shader
            ambientShader.Use (drawCallback);

            // Set up blending and depth testing
            GL.Enable (EnableCap.Blend);
            GL.BlendFunc (BlendingFactorSrc.One, BlendingFactorDest.One);
            GL.DepthMask (false);
            GL.DepthFunc (DepthFunction.Equal);

            // Set the eye position
            directionalShader ["eye_pos"] = camera.Position;

            // Iterate over the directional lights
            foreach (var light in directionalLights) {

                // Set the uniforms
                directionalShader ["directionalLight.base.color"] = light.ColorToVector3 ();
                directionalShader ["directionalLight.base.intensity"] = light.Intensity;
                directionalShader ["directionalLight.direction"] = light.Direction;

                // Draw the scene using the directional shader
                directionalShader.Use (drawCallback);
            }

            // Reset blending and depth testing
            GL.DepthFunc (DepthFunction.Less);
            GL.DepthMask (true);
            GL.Disable (EnableCap.Blend);
        }

        /// <summary>
        /// Draws the specified model using the render pipeline.
        /// </summary>
        /// <param name="camera">Camera.</param>
        /// <param name="model">Model.</param>
        public void Draw (Camera camera, Model model) {

            // Create the draw callback
            Action<ShaderProgram> callback = (shader) => {
                model.Draw (shader, camera);
            };

            // Draw the model using the render pipeline
            Draw (camera, callback);
        }

        /// <summary>
        /// Draws the specified model using the render pipeline.
        /// </summary>
        /// <param name="camera">Camera.</param>
        /// <param name="multiModel">Multi model.</param>
        public void Draw (Camera camera, MultiModel multiModel) {

            // Create the draw callback
            Action<ShaderProgram> callback = (shader) => {
                foreach (var model in multiModel) {
                    model.Draw (shader, camera);
                }
            };

            // Draw the model using the render pipeline
            Draw (camera, callback);
        }

        public void Display (Viewport viewport = null) {

            // Get the target viewport
            var viewPort = viewport ?? game.Viewport;

            // Bind the color texture of the frame buffer
            frameBuffer.ColorTexture.Bind ();

            // Draw the color texture of the frame buffer
            viewPort.Draw (frameBuffer.ColorTexture);
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose (bool disposing) {
            if (!disposedValue) {
                if (disposing) {
                    ambientShader.Dispose ();
                    directionalShader.Dispose ();
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

