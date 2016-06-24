using System;
using System.Runtime.InteropServices;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace libzeta {

    /// <summary>
    /// Sprite batch.
    /// </summary>
    public sealed class SpriteBatch : IDisposable {

        /// <summary>
        /// The max amount of batched sprites.
        /// </summary>
        const int MAX_BATCHES = 2048;

        /// <summary>
        /// The max amount of batched vertices.
        /// </summary>
        const int MAX_VERTICES = MAX_BATCHES * 4;

        /// <summary>
        /// The max amount of batched indices.
        /// </summary>
        const int MAX_INDICES = MAX_BATCHES * 6;

        /// <summary>
        /// The game.
        /// </summary>
        readonly Game game;

        /// <summary>
        /// The shader program.
        /// </summary>
        readonly ShaderProgram shader;

        /// <summary>
        /// The vertex buffer object.
        /// </summary>
        readonly GLBufferDynamic<Vertex2> vbo;

        /// <summary>
        /// The index buffer object.
        /// </summary>
        readonly GLBuffer<uint> ibo;

        /// <summary>
        /// The vertices.
        /// </summary>
        Vertex2 [] vertices;

        /// <summary>
        /// The array buffer object.
        /// </summary>
        int abo = -1;

        /// <summary>
        /// The vertex count.
        /// </summary>
        int vertexCount;

        /// <summary>
        /// The index count.
        /// </summary>
        int indexCount;

        /// <summary>
        /// Whether the sprite batch is currently active.
        /// </summary>
        bool active;

        /// <summary>
        /// The internal camera.
        /// </summary>
        Camera internalCamera;

        /// <summary>
        /// The currently active camera.
        /// </summary>
        Camera currentCamera;

        /// <summary>
        /// The currently active texture.
        /// </summary>
        Texture2D currentTexture;

        /// <summary>
        /// The frame buffer.
        /// </summary>
        FrameBuffer frameBuffer;

        /// <summary>
        /// Gets whether the sprite batch is currently active.
        /// </summary>
        /// <value>Whether the sprite batch is currently active.</value>
        public bool Active => active;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:libzeta.SpriteBatch"/> class.
        /// </summary>
        /// <param name="game">Game.</param>
        /// <param name="shaderProgram">Shader program.</param>
        public SpriteBatch (Game game, ShaderProgram shaderProgram = null) {

            // Set the game
            this.game = game;

            // Set the shader program
            if (shaderProgram == null) {
                var vertShader = new VertexShader (ShaderCollection.SpriteBatch.VertexShader);
                var fragShader = new FragmentShader (ShaderCollection.SpriteBatch.FragmentShader);
                shaderProgram = new ShaderProgram (vertShader, fragShader);
                shaderProgram.Link ();
            }
            shader = shaderProgram;

            // Create the buffer settings
            var settings = new GLBufferSettings {
                Hint = BufferUsageHint.DynamicDraw,
                Target = BufferTarget.ArrayBuffer,
                Type = VertexAttribPointerType.Float
            };

            // Create temporary index array
            var indPtr = 0;
            var indTmp = new uint [MAX_INDICES];
            for (uint i = 0; i < MAX_VERTICES; i += 4) {

                // Triangle 1
                indTmp [indPtr++] = i + 0;
                indTmp [indPtr++] = i + 1;
                indTmp [indPtr++] = i + 2;

                // Triangle 2
                indTmp [indPtr++] = i + 1;
                indTmp [indPtr++] = i + 3;
                indTmp [indPtr++] = i + 2;
            }

            // Create the array buffer object
            abo = GL.GenVertexArray ();

            // Create the vertex buffer object
            vbo = new GLBufferDynamic<Vertex2> (
                settings: settings,
                elementSize: Marshal.SizeOf (typeof (Vertex2)),
                startCapacity: MAX_VERTICES
            );

            // Create the index buffer object
            ibo = new GLBuffer<uint> (
                settings: GLBufferSettings.DynamicIndices,
                buffer: indTmp
            );

            // Initialize the vertices
            vertices = new Vertex2 [MAX_VERTICES];

            // Initialize the texture
            currentTexture = Texture2D.Transparent;

            // Create the internal camera
            var resolution = new Resolution (game.Window.Width, game.Window.Height);
            internalCamera = new OrthographicCamera (resolution);

            // Create the frame buffer
            frameBuffer = new FrameBuffer (
                width: resolution.Width,
                height: resolution.Height,
                target: FramebufferTarget.Framebuffer
            );
        }

        public void Begin () {
            Begin (internalCamera);
        }

        public void Begin (Camera camera) {

            // Test if the sprite batch is active
            if (active) {
                throw new InvalidOperationException ("Cannot begin active SpriteBatch!");
            }

            // Mark the sprite batch as active
            active = true;

            // Reset the vertex and index counts
            vertexCount = 0;
            indexCount = 0;

            // Set the camera
            currentCamera = camera;

            // Bind the frame buffer
            frameBuffer.Bind ();

            // Clear the color and depth buffers of the frame buffer
            GL.Clear (ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }

        public void End () {

            // Test if the sprite batch is inactive
            if (!active) {
                throw new InvalidOperationException ("Cannot end inactive SpriteBatch!");
            }
            
            // Flush the textures
            Flush ();

            // Unbind the frame buffer
            frameBuffer.Unbind ();

            // Draw the color texture of the frame buffer
            game.Viewport.Draw (frameBuffer.ColorTexture);

            // Mark the sprite batch as inactive
            active = false;
        }

        public void TryBegin () {
            TryBegin (internalCamera);
        }

        public void TryBegin (Camera camera) {
            if (!active)
                Begin (camera);
        }

        public void TryEnd () {
            if (active)
                End ();
        }

        void Flush () {

            // Return if there is nothing to be drawn
            if (indexCount == 0)
                return;

            // Use the shader program
            shader.Use (shader => {

                // Bind the array buffer object
                GL.BindVertexArray (abo);

                // Upload vertices to the vertex buffer object
                vbo.UploadData (dataArray: vertices);

                // Point the vertex buffer object to the right point
                vbo.PointTo (shader.Attrib ("v_pos"), 2, 0);
                vbo.PointTo (shader.Attrib ("v_tex"), 2, 3 * sizeof (float));
                vbo.PointTo (shader.Attrib ("v_col"), 4, 5 * sizeof (float));

                // Bind the current texture to texture unit 0
                currentTexture.Bind (TextureUnit.Texture0);

                // Bind the index buffer object
                ibo.Bind ();

                // Set the MVP uniform to the view projection matrix of the camera
                shader ["MVP"] = currentCamera.ViewProjection;

                // Draw the elements
                GL.DrawElements (BeginMode.Triangles, ibo.Buffer.Count, DrawElementsType.UnsignedInt, 0);

                // Unbind the array buffer object
                GL.BindVertexArray (0);

                // Clear the vertices
                Array.Clear (vertices, 0, vertices.Length);

                // Reset the vertex and index counts
                vertexCount = 0;
                indexCount = 0;
            });
        }

        public void Draw (Texture2D texture, Point2 destLocation) {

            // Draw the texture
            DrawInternal (
                texture: texture,
                sourceRect: texture.Bounds,
                destRect: new Rectangle (destLocation.X, destLocation.Y, texture.Width, texture.Height),
                tint: Color4.White,
                scale: Vector2.One,
                dx: 0,
                dy: 0,
                depth: 0,
                rot: 0
            );
        }

        public void Draw (Texture2D texture, Point2 destLocation, Color4 tint) {

            // Draw the texture
            DrawInternal (
                texture: texture,
                sourceRect: texture.Bounds,
                destRect: new Rectangle (destLocation.X, destLocation.Y, texture.Width, texture.Height),
                tint: tint,
                scale: Vector2.One,
                dx: 0,
                dy: 0,
                depth: 0,
                rot: 0
            );
        }

        public void Draw (Texture2D texture, Point2 destLocation, float rotation) {

            // Draw the texture
            DrawInternal (
                texture: texture,
                sourceRect: texture.Bounds,
                destRect: new Rectangle (destLocation.X, destLocation.Y, texture.Width, texture.Height),
                tint: Color4.White,
                scale: Vector2.One,
                dx: 0,
                dy: 0,
                depth: 0,
                rot: rotation
            );
        }

        public void Draw (Texture2D texture, Point2 destLocation, float rotation, Color4 tint) {

            // Draw the texture
            DrawInternal (
                texture: texture,
                sourceRect: texture.Bounds,
                destRect: new Rectangle (destLocation.X, destLocation.Y, texture.Width, texture.Height),
                tint: tint,
                scale: Vector2.One,
                dx: 0,
                dy: 0,
                depth: 0,
                rot: rotation
            );
        }

        public void Draw (Texture2D texture, Point2 destLocation, int depth) {

            // Draw the texture
            DrawInternal (
                texture: texture,
                sourceRect: texture.Bounds,
                destRect: new Rectangle (destLocation.X, destLocation.Y, texture.Width, texture.Height),
                tint: Color4.White,
                scale: Vector2.One,
                dx: 0,
                dy: 0,
                depth: depth,
                rot: 0
            );
        }

        public void Draw (Texture2D texture, Point2 destLocation, int depth, Color4 tint) {

            // Draw the texture
            DrawInternal (
                texture: texture,
                sourceRect: texture.Bounds,
                destRect: new Rectangle (destLocation.X, destLocation.Y, texture.Width, texture.Height),
                tint: tint,
                scale: Vector2.One,
                dx: 0,
                dy: 0,
                depth: depth,
                rot: 0
            );
        }

        public void Draw (Texture2D texture, Point2 destLocation, int depth, float rotation) {

            // Draw the texture
            DrawInternal (
                texture: texture,
                sourceRect: texture.Bounds,
                destRect: new Rectangle (destLocation.X, destLocation.Y, texture.Width, texture.Height),
                tint: Color4.White,
                scale: Vector2.One,
                dx: 0,
                dy: 0,
                depth: depth,
                rot: rotation
            );
        }

        public void Draw (Texture2D texture, Point2 destLocation, int depth, float rotation, Color4 tint) {

            // Draw the texture
            DrawInternal (
                texture: texture,
                sourceRect: texture.Bounds,
                destRect: new Rectangle (destLocation.X, destLocation.Y, texture.Width, texture.Height),
                tint: tint,
                scale: Vector2.One,
                dx: 0,
                dy: 0,
                depth: depth,
                rot: rotation
            );
        }

        public void Draw (Texture2D texture, Point2 destLocation, Scale scale) {

            // Draw the texture
            DrawInternal (
                texture: texture,
                sourceRect: texture.Bounds,
                destRect: new Rectangle (destLocation.X, destLocation.Y, texture.Width, texture.Height),
                tint: Color4.White,
                scale: new Vector2 (scale.ScaleX, scale.ScaleY),
                dx: 0,
                dy: 0,
                depth: 0,
                rot: 0
            );
        }

        public void Draw (Texture2D texture, Point2 destLocation, Scale scale, Color4 tint) {

            // Draw the texture
            DrawInternal (
                texture: texture,
                sourceRect: texture.Bounds,
                destRect: new Rectangle (destLocation.X, destLocation.Y, texture.Width, texture.Height),
                tint: tint,
                scale: new Vector2 (scale.ScaleX, scale.ScaleY),
                dx: 0,
                dy: 0,
                depth: 0,
                rot: 0
            );
        }

        public void Draw (Texture2D texture, Point2 destLocation, Scale scale, float rotation) {

            // Draw the texture
            DrawInternal (
                texture: texture,
                sourceRect: texture.Bounds,
                destRect: new Rectangle (destLocation.X, destLocation.Y, texture.Width, texture.Height),
                tint: Color4.White,
                scale: new Vector2 (scale.ScaleX, scale.ScaleY),
                dx: 0,
                dy: 0,
                depth: 0,
                rot: rotation
            );
        }

        public void Draw (Texture2D texture, Point2 destLocation, Scale scale, float rotation, Color4 tint) {

            // Draw the texture
            DrawInternal (
                texture: texture,
                sourceRect: texture.Bounds,
                destRect: new Rectangle (destLocation.X, destLocation.Y, texture.Width, texture.Height),
                tint: tint,
                scale: new Vector2 (scale.ScaleX, scale.ScaleY),
                dx: 0,
                dy: 0,
                depth: 0,
                rot: rotation
            );
        }

        public void Draw (Texture2D texture, Point2 destLocation, Scale scale, int depth) {

            // Draw the texture
            DrawInternal (
                texture: texture,
                sourceRect: texture.Bounds,
                destRect: new Rectangle (destLocation.X, destLocation.Y, texture.Width, texture.Height),
                tint: Color4.White,
                scale: new Vector2 (scale.ScaleX, scale.ScaleY),
                dx: 0,
                dy: 0,
                depth: depth * scale.ScaleZ,
                rot: 0
            );
        }

        public void Draw (Texture2D texture, Point2 destLocation, Scale scale, int depth, Color4 tint) {

            // Draw the texture
            DrawInternal (
                texture: texture,
                sourceRect: texture.Bounds,
                destRect: new Rectangle (destLocation.X, destLocation.Y, texture.Width, texture.Height),
                tint: tint,
                scale: new Vector2 (scale.ScaleX, scale.ScaleY),
                dx: 0,
                dy: 0,
                depth: depth * scale.ScaleZ,
                rot: 0
            );
        }

        public void Draw (Texture2D texture, Point2 destLocation, Scale scale, int depth, float rotation) {

            // Draw the texture
            DrawInternal (
                texture: texture,
                sourceRect: texture.Bounds,
                destRect: new Rectangle (destLocation.X, destLocation.Y, texture.Width, texture.Height),
                tint: Color4.White,
                scale: new Vector2 (scale.ScaleX, scale.ScaleY),
                dx: 0,
                dy: 0,
                depth: depth * scale.ScaleZ,
                rot: rotation
            );
        }

        public void Draw (Texture2D texture, Point2 destLocation, Scale scale, int depth, float rotation, Color4 tint) {

            // Draw the texture
            DrawInternal (
                texture: texture,
                sourceRect: texture.Bounds,
                destRect: new Rectangle (destLocation.X, destLocation.Y, texture.Width, texture.Height),
                tint: tint,
                scale: new Vector2 (scale.ScaleX, scale.ScaleY),
                dx: 0,
                dy: 0,
                depth: depth * scale.ScaleZ,
                rot: rotation
            );
        }

        public void Draw (Texture2D texture, Point2 destLocation, Point2 origin) {

            // Draw the texture
            DrawInternal (
                texture: texture,
                sourceRect: texture.Bounds,
                destRect: new Rectangle (destLocation.X, destLocation.Y, texture.Width, texture.Height),
                tint: Color4.White,
                scale: Vector2.One,
                dx: -origin.X,
                dy: -origin.Y,
                depth: 0,
                rot: 0
            );
        }

        public void Draw (Texture2D texture, Point2 destLocation, Point2 origin, Color4 tint) {

            // Draw the texture
            DrawInternal (
                texture: texture,
                sourceRect: texture.Bounds,
                destRect: new Rectangle (destLocation.X, destLocation.Y, texture.Width, texture.Height),
                tint: tint,
                scale: Vector2.One,
                dx: -origin.X,
                dy: -origin.Y,
                depth: 0,
                rot: 0
            );
        }

        public void Draw (Texture2D texture, Point2 destLocation, Point2 origin, float rotation) {

            // Draw the texture
            DrawInternal (
                texture: texture,
                sourceRect: texture.Bounds,
                destRect: new Rectangle (destLocation.X, destLocation.Y, texture.Width, texture.Height),
                tint: Color4.White,
                scale: Vector2.One,
                dx: -origin.X,
                dy: -origin.Y,
                depth: 0,
                rot: rotation
            );
        }

        public void Draw (Texture2D texture, Point2 destLocation, Point2 origin, float rotation, Color4 tint) {

            // Draw the texture
            DrawInternal (
                texture: texture,
                sourceRect: texture.Bounds,
                destRect: new Rectangle (destLocation.X, destLocation.Y, texture.Width, texture.Height),
                tint: tint,
                scale: Vector2.One,
                dx: -origin.X,
                dy: -origin.Y,
                depth: 0,
                rot: rotation
            );
        }

        public void Draw (Texture2D texture, Point2 destLocation, Point2 origin, int depth) {

            // Draw the texture
            DrawInternal (
                texture: texture,
                sourceRect: texture.Bounds,
                destRect: new Rectangle (destLocation.X, destLocation.Y, texture.Width, texture.Height),
                tint: Color4.White,
                scale: Vector2.One,
                dx: -origin.X,
                dy: -origin.Y,
                depth: depth,
                rot: 0
            );
        }

        public void Draw (Texture2D texture, Point2 destLocation, Point2 origin, int depth, Color4 tint) {

            // Draw the texture
            DrawInternal (
                texture: texture,
                sourceRect: texture.Bounds,
                destRect: new Rectangle (destLocation.X, destLocation.Y, texture.Width, texture.Height),
                tint: tint,
                scale: Vector2.One,
                dx: -origin.X,
                dy: -origin.Y,
                depth: depth,
                rot: 0
            );
        }

        public void Draw (Texture2D texture, Point2 destLocation, Point2 origin, int depth, float rotation) {

            // Draw the texture
            DrawInternal (
                texture: texture,
                sourceRect: texture.Bounds,
                destRect: new Rectangle (destLocation.X, destLocation.Y, texture.Width, texture.Height),
                tint: Color4.White,
                scale: Vector2.One,
                dx: -origin.X,
                dy: -origin.Y,
                depth: depth,
                rot: rotation
            );
        }

        public void Draw (Texture2D texture, Point2 destLocation, Point2 origin, int depth, float rotation, Color4 tint) {

            // Draw the texture
            DrawInternal (
                texture: texture,
                sourceRect: texture.Bounds,
                destRect: new Rectangle (destLocation.X, destLocation.Y, texture.Width, texture.Height),
                tint: tint,
                scale: Vector2.One,
                dx: -origin.X,
                dy: -origin.Y,
                depth: depth,
                rot: rotation
            );
        }

        public void Draw (Texture2D texture, Point2 destLocation, Point2 origin, Scale scale) {

            // Draw the texture
            DrawInternal (
                texture: texture,
                sourceRect: texture.Bounds,
                destRect: new Rectangle (destLocation.X, destLocation.Y, texture.Width, texture.Height),
                tint: Color4.White,
                scale: new Vector2 (scale.ScaleX, scale.ScaleY),
                dx: -origin.X,
                dy: -origin.Y,
                depth: 0,
                rot: 0
            );
        }

        public void Draw (Texture2D texture, Point2 destLocation, Point2 origin, Scale scale, Color4 tint) {

            // Draw the texture
            DrawInternal (
                texture: texture,
                sourceRect: texture.Bounds,
                destRect: new Rectangle (destLocation.X, destLocation.Y, texture.Width, texture.Height),
                tint: tint,
                scale: new Vector2 (scale.ScaleX, scale.ScaleY),
                dx: -origin.X,
                dy: -origin.Y,
                depth: 0,
                rot: 0
            );
        }

        public void Draw (Texture2D texture, Point2 destLocation, Point2 origin, Scale scale, float rotation) {

            // Draw the texture
            DrawInternal (
                texture: texture,
                sourceRect: texture.Bounds,
                destRect: new Rectangle (destLocation.X, destLocation.Y, texture.Width, texture.Height),
                tint: Color4.White,
                scale: new Vector2 (scale.ScaleX, scale.ScaleY),
                dx: -origin.X,
                dy: -origin.Y,
                depth: 0,
                rot: rotation
            );
        }

        public void Draw (Texture2D texture, Point2 destLocation, Point2 origin, Scale scale, float rotation, Color4 tint) {

            // Draw the texture
            DrawInternal (
                texture: texture,
                sourceRect: texture.Bounds,
                destRect: new Rectangle (destLocation.X, destLocation.Y, texture.Width, texture.Height),
                tint: tint,
                scale: new Vector2 (scale.ScaleX, scale.ScaleY),
                dx: -origin.X,
                dy: -origin.Y,
                depth: 0,
                rot: rotation
            );
        }

        public void Draw (Texture2D texture, Point2 destLocation, Point2 origin, Scale scale, int depth) {

            // Draw the texture
            DrawInternal (
                texture: texture,
                sourceRect: texture.Bounds,
                destRect: new Rectangle (destLocation.X, destLocation.Y, texture.Width, texture.Height),
                tint: Color4.White,
                scale: new Vector2 (scale.ScaleX, scale.ScaleY),
                dx: -origin.X,
                dy: -origin.Y,
                depth: depth * scale.ScaleZ,
                rot: 0
            );
        }

        public void Draw (Texture2D texture, Point2 destLocation, Point2 origin, Scale scale, int depth, Color4 tint) {

            // Draw the texture
            DrawInternal (
                texture: texture,
                sourceRect: texture.Bounds,
                destRect: new Rectangle (destLocation.X, destLocation.Y, texture.Width, texture.Height),
                tint: tint,
                scale: new Vector2 (scale.ScaleX, scale.ScaleY),
                dx: -origin.X,
                dy: -origin.Y,
                depth: depth * scale.ScaleZ,
                rot: 0
            );
        }

        public void Draw (Texture2D texture, Point2 destLocation, Point2 origin, Scale scale, int depth, float rotation) {

            // Draw the texture
            DrawInternal (
                texture: texture,
                sourceRect: texture.Bounds,
                destRect: new Rectangle (destLocation.X, destLocation.Y, texture.Width, texture.Height),
                tint: Color4.White,
                scale: new Vector2 (scale.ScaleX, scale.ScaleY),
                dx: -origin.X,
                dy: -origin.Y,
                depth: depth * scale.ScaleZ,
                rot: rotation
            );
        }

        public void Draw (Texture2D texture, Point2 destLocation, Point2 origin, Scale scale, int depth, float rotation, Color4 tint) {

            // Draw the texture
            DrawInternal (
                texture: texture,
                sourceRect: texture.Bounds,
                destRect: new Rectangle (destLocation.X, destLocation.Y, texture.Width, texture.Height),
                tint: tint,
                scale: new Vector2 (scale.ScaleX, scale.ScaleY),
                dx: -origin.X,
                dy: -origin.Y,
                depth: depth * scale.ScaleZ,
                rot: rotation
            );
        }

        public void Draw (Texture2D texture, Rectangle sourceRect, Rectangle destRect) {

            // Draw the texture
            DrawInternal (
                texture: texture,
                sourceRect: sourceRect,
                destRect: destRect,
                tint: Color4.White,
                scale: Vector2.One,
                dx: 0,
                dy: 0,
                depth: 0,
                rot: 0
            );
        }

        public void Draw (Texture2D texture, Rectangle sourceRect, Rectangle destRect, int depth) {

            // Draw the texture
            DrawInternal (
                texture: texture,
                sourceRect: sourceRect,
                destRect: destRect,
                tint: Color4.White,
                scale: Vector2.One,
                dx: 0,
                dy: 0,
                depth: depth,
                rot: 0
            );
        }

        public void Draw (Texture2D texture, Rectangle sourceRect, Rectangle destRect, float rotation) {

            // Draw the texture
            DrawInternal (
                texture: texture,
                sourceRect: sourceRect,
                destRect: destRect,
                tint: Color4.White,
                scale: Vector2.One,
                dx: 0,
                dy: 0,
                depth: 0,
                rot: rotation
            );
        }

        public void Draw (Texture2D texture, Rectangle sourceRect, Rectangle destRect, Color4 tint) {

            // Draw the texture
            DrawInternal (
                texture: texture,
                sourceRect: sourceRect,
                destRect: destRect,
                tint: tint,
                scale: Vector2.One,
                dx: 0,
                dy: 0,
                depth: 0,
                rot: 0
            );
        }

        public void Draw (Texture2D texture, Rectangle sourceRect, Rectangle destRect, int depth, Color4 tint) {

            // Draw the texture
            DrawInternal (
                texture: texture,
                sourceRect: sourceRect,
                destRect: destRect,
                tint: tint,
                scale: Vector2.One,
                dx: 0,
                dy: 0,
                depth: depth,
                rot: 0
            );
        }

        public void Draw (Texture2D texture, Rectangle sourceRect, Rectangle destRect, int depth, float rotation) {

            // Draw the texture
            DrawInternal (
                texture: texture,
                sourceRect: sourceRect,
                destRect: destRect,
                tint: Color4.White,
                scale: Vector2.One,
                dx: 0,
                dy: 0,
                depth: depth,
                rot: rotation
            );
        }

        public void Draw (Texture2D texture, Rectangle sourceRect, Rectangle destRect, int depth, float rotation, Color4 tint) {

            // Draw the texture
            DrawInternal (
                texture: texture,
                sourceRect: sourceRect,
                destRect: destRect,
                tint: tint,
                scale: Vector2.One,
                dx: 0,
                dy: 0,
                depth: depth,
                rot: rotation
            );
        }

        public void Draw (Texture2D texture, Rectangle sourceRect, Rectangle destRect, float rotation, Color4 tint) {

            // Draw the texture
            DrawInternal (
                texture: texture,
                sourceRect: sourceRect,
                destRect: destRect,
                tint: tint,
                scale: Vector2.One,
                dx: 0,
                dy: 0,
                depth: 0,
                rot: rotation
            );
        }

        public void Draw (Texture2D texture, Rectangle sourceRect, Rectangle destRect, Point2 origin) {

            // Draw the texture
            DrawInternal (
                texture: texture,
                sourceRect: sourceRect,
                destRect: destRect,
                tint: Color4.White,
                scale: Vector2.One,
                dx: -origin.X,
                dy: -origin.Y,
                depth: 0,
                rot: 0
            );
        }

        public void Draw (Texture2D texture, Rectangle sourceRect, Rectangle destRect, Point2 origin, Color4 tint) {

            // Draw the texture
            DrawInternal (
                texture: texture,
                sourceRect: sourceRect,
                destRect: destRect,
                tint: tint,
                scale: Vector2.One,
                dx: -origin.X,
                dy: -origin.Y,
                depth: 0,
                rot: 0
            );
        }

        public void Draw (Texture2D texture, Rectangle sourceRect, Rectangle destRect, Point2 origin, float rotation) {

            // Draw the texture
            DrawInternal (
                texture: texture,
                sourceRect: sourceRect,
                destRect: destRect,
                tint: Color4.White,
                scale: Vector2.One,
                dx: -origin.X,
                dy: -origin.Y,
                depth: 0,
                rot: rotation
            );
        }

        public void Draw (Texture2D texture, Rectangle sourceRect, Rectangle destRect, Point2 origin, float rotation, Color4 tint) {

            // Draw the texture
            DrawInternal (
                texture: texture,
                sourceRect: sourceRect,
                destRect: destRect,
                tint: tint,
                scale: Vector2.One,
                dx: -origin.X,
                dy: -origin.Y,
                depth: 0,
                rot: rotation
            );
        }

        public void Draw (Texture2D texture, Rectangle sourceRect, Rectangle destRect, Point2 origin, int depth) {

            // Draw the texture
            DrawInternal (
                texture: texture,
                sourceRect: sourceRect,
                destRect: destRect,
                tint: Color4.White,
                scale: Vector2.One,
                dx: -origin.X,
                dy: -origin.Y,
                depth: depth,
                rot: 0
            );
        }

        public void Draw (Texture2D texture, Rectangle sourceRect, Rectangle destRect, Point2 origin, int depth, Color4 tint) {

            // Draw the texture
            DrawInternal (
                texture: texture,
                sourceRect: sourceRect,
                destRect: destRect,
                tint: tint,
                scale: Vector2.One,
                dx: -origin.X,
                dy: -origin.Y,
                depth: depth,
                rot: 0
            );
        }

        public void Draw (Texture2D texture, Rectangle sourceRect, Rectangle destRect, Point2 origin, int depth, float rotation) {

            // Draw the texture
            DrawInternal (
                texture: texture,
                sourceRect: sourceRect,
                destRect: destRect,
                tint: Color4.White,
                scale: Vector2.One,
                dx: -origin.X,
                dy: -origin.Y,
                depth: depth,
                rot: rotation
            );
        }

        public void Draw (Texture2D texture, Rectangle sourceRect, Rectangle destRect, Point2 origin, int depth, float rotation, Color4 tint) {

            // Draw the texture
            DrawInternal (
                texture: texture,
                sourceRect: sourceRect,
                destRect: destRect,
                tint: tint,
                scale: Vector2.One,
                dx: -origin.X,
                dy: -origin.Y,
                depth: depth,
                rot: rotation
            );
        }

        public void Draw (Texture2D texture, Rectangle sourceRect, Rectangle destRect, Point2 origin, Scale scale) {

            // Draw the texture
            DrawInternal (
                texture: texture,
                sourceRect: sourceRect,
                destRect: destRect,
                tint: Color4.White,
                scale: new Vector2 (scale.ScaleX, scale.ScaleY),
                dx: -origin.X,
                dy: -origin.Y,
                depth: 0,
                rot: 0
            );
        }

        public void Draw (Texture2D texture, Rectangle sourceRect, Rectangle destRect, Point2 origin, Scale scale, Color4 tint) {

            // Draw the texture
            DrawInternal (
                texture: texture,
                sourceRect: sourceRect,
                destRect: destRect,
                tint: tint,
                scale: new Vector2 (scale.ScaleX, scale.ScaleY),
                dx: -origin.X,
                dy: -origin.Y,
                depth: 0,
                rot: 0
            );
        }

        public void Draw (Texture2D texture, Rectangle sourceRect, Rectangle destRect, Point2 origin, Scale scale, float rotation) {

            // Draw the texture
            DrawInternal (
                texture: texture,
                sourceRect: sourceRect,
                destRect: destRect,
                tint: Color4.White,
                scale: new Vector2 (scale.ScaleX, scale.ScaleY),
                dx: -origin.X,
                dy: -origin.Y,
                depth: 0,
                rot: rotation
            );
        }

        public void Draw (Texture2D texture, Rectangle sourceRect, Rectangle destRect, Point2 origin, Scale scale, float rotation, Color4 tint) {

            // Draw the texture
            DrawInternal (
                texture: texture,
                sourceRect: sourceRect,
                destRect: destRect,
                tint: tint,
                scale: new Vector2 (scale.ScaleX, scale.ScaleY),
                dx: -origin.X,
                dy: -origin.Y,
                depth: 0,
                rot: rotation
            );
        }

        public void Draw (Texture2D texture, Rectangle sourceRect, Rectangle destRect, Point2 origin, Scale scale, int depth) {

            // Draw the texture
            DrawInternal (
                texture: texture,
                sourceRect: sourceRect,
                destRect: destRect,
                tint: Color4.White,
                scale: new Vector2 (scale.ScaleX, scale.ScaleY),
                dx: -origin.X,
                dy: -origin.Y,
                depth: depth,
                rot: 0
            );
        }

        public void Draw (Texture2D texture, Rectangle sourceRect, Rectangle destRect, Point2 origin, Scale scale, int depth, Color4 tint) {

            // Draw the texture
            DrawInternal (
                texture: texture,
                sourceRect: sourceRect,
                destRect: destRect,
                tint: tint,
                scale: new Vector2 (scale.ScaleX, scale.ScaleY),
                dx: -origin.X,
                dy: -origin.Y,
                depth: depth,
                rot: 0
            );
        }

        public void Draw (Texture2D texture, Rectangle sourceRect, Rectangle destRect, Point2 origin, Scale scale, int depth, float rotation) {

            // Draw the texture
            DrawInternal (
                texture: texture,
                sourceRect: sourceRect,
                destRect: destRect,
                tint: Color4.White,
                scale: new Vector2 (scale.ScaleX, scale.ScaleY),
                dx: -origin.X,
                dy: -origin.Y,
                depth: depth,
                rot: rotation
            );
        }

        public void Draw (Texture2D texture, Rectangle sourceRect, Rectangle destRect, Point2 origin, Scale scale, int depth, float rotation, Color4 tint) {

            // Draw the texture
            DrawInternal (
                texture: texture,
                sourceRect: sourceRect,
                destRect: destRect,
                tint: tint,
                scale: new Vector2 (scale.ScaleX, scale.ScaleY),
                dx: -origin.X,
                dy: -origin.Y,
                depth: depth,
                rot: rotation
            );
        }

        void DrawInternal (Texture2D texture, Rectangle? sourceRect, Rectangle destRect, Color4 tint, Vector2 scale, float dx, float dy, float depth, float rot) {

            // Flush if the current texture is valid
            // and the new texture differs from the current texture
            if (currentTexture.TextureId != -1 && texture.TextureId != currentTexture.TextureId)
                Flush ();

            // Set the current texture to the new texture
            currentTexture = texture;

            // Flush if the vertex or index counts exceeds the maximum
            if (indexCount + 6 >= MAX_INDICES || vertexCount + 4 >= MAX_VERTICES)
                Flush ();

            // Construct source rectangle
            Rectangle source = sourceRect ?? new Rectangle (0, 0, texture.Width, texture.Height);

            var pos = new Vector2 (destRect.X, destRect.Y);
            var size = new Vector2 (destRect.Width * scale.X, destRect.Height * scale.Y);

            var sin = (float)Math.Sin (rot);
            var cos = (float)Math.Cos (rot);

            float x = pos.X;
            float y = pos.Y;
            float w = size.X;
            float h = size.Y;

            // Top left
            vertices [vertexCount++] = new Vertex2 (
                pos: new Vector3 (
                    x + dx * cos - dy * sin,
                    y + dx * sin + dy * cos,
                    z: depth
                ),
                texcoord:
                new Vector2 (
                    x: source.X / (float)texture.Width,
                    y: source.Y / (float)texture.Height
                ),
                color: tint
            );

            // Top right
            vertices [vertexCount++] = new Vertex2 (
                pos: new Vector3 (
                    x + (dx + w) * cos - dy * sin,
                    y + (dx + w) * sin + dy * cos,
                    z: depth
                ),
                texcoord: new Vector2 (
                    x: (source.X + source.Width) / (float)texture.Width,
                    y: source.Y / (float)texture.Height
                ),
                color: tint
            );

            // Bottom left
            vertices [vertexCount++] = new Vertex2 (
                pos: new Vector3 (
                    x + dx * cos - (dy + h) * sin,
                    y + dx * sin + (dy + h) * cos,
                    z: depth
                ),
                texcoord: new Vector2 (
                    x: source.X / (float)texture.Width,
                    y: (source.Y + source.Height) / (float)texture.Height
                ),
                color: tint
            );

            // Bottom right
            vertices [vertexCount++] = new Vertex2 (
                pos: new Vector3 (
                    x + (dx + w) * cos - (dy + h) * sin,
                    y + (dx + w) * sin + (dy + h) * cos,
                    z: depth
                ),
                texcoord: new Vector2 (
                    x: (source.X + source.Width) / (float)texture.Width,
                    y: (source.Y + source.Height) / (float)texture.Height
                ),
                color: tint
            );

            // Increment index count
            indexCount += 6;
        }

        #region IDisposable Support
        bool disposedValue = false;

        void Dispose (bool disposing) {
            if (!disposedValue) {
                if (disposing) {
                    shader.Dispose ();
                }
                vertices = null;
                disposedValue = true;
            }
        }

        public void Dispose () {
            Dispose (true);
        }
        #endregion
    }
}

