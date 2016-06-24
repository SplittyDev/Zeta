using System;
using OpenTK.Graphics.OpenGL4;

namespace libzeta {

    /// <summary>
    /// Frame buffer.
    /// </summary>
    public class FrameBuffer : IBindable {

        /// <summary>
        /// The buffer identifier.
        /// </summary>
        readonly int bufferId;

        /// <summary>
        /// The target.
        /// </summary>
        readonly FramebufferTarget target;

        /// <summary>
        /// The color texture.
        /// </summary>
        public Texture2D ColorTexture;

        /// <summary>
        /// The depth texture.
        /// </summary>
        public Texture2D DepthTexture;

        /// <summary>
        /// The width.
        /// </summary>
        public int Width;

        /// <summary>
        /// The height.
        /// </summary>
        public int Height;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:libzeta.Framebuffer"/> class.
        /// </summary>
        /// <param name="target">Target.</param>
        FrameBuffer (FramebufferTarget target) {
            this.target = target;
            bufferId = GL.GenFramebuffer ();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:libzeta.Framebuffer"/> class.
        /// </summary>
        /// <param name="width">Width.</param>
        /// <param name="height">Height.</param>
        /// <param name="target">Target.</param>
        public FrameBuffer (int width, int height, FramebufferTarget target) : this (target) {

            // Set the width and height
            Width = width;
            Height = height;

            // Create the color texture
            ColorTexture = new Texture2D (
                target: TextureTarget.Texture2D,
                internalFormat: PixelInternalFormat.Rgba,
                format: PixelFormat.Rgba,
                type: PixelType.UnsignedByte,
                mode: InterpolationMode.Linear,
                mipmap: false,
                width: width,
                height: height
            );

            // Create the depth texture
            DepthTexture = new Texture2D (
                target: TextureTarget.Texture2D,
                internalFormat: PixelInternalFormat.DepthComponent32,
                format: PixelFormat.DepthComponent,
                type: PixelType.Float,
                mode: InterpolationMode.Linear,
                mipmap: false,
                width: width,
                height: height
            );

            // Bind the frame buffer
            Bind ();

            // Set the color attachment up
            ColorTexture.Bind ();
            GL.FramebufferTexture (FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, ColorTexture.TextureId, 0);

            // Set the depth attachment up
            DepthTexture.Bind ();
            GL.FramebufferTexture (FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, DepthTexture.TextureId, 0);

            // Set the draw buffer up
            GL.DrawBuffer (DrawBufferMode.ColorAttachment0);

            // Unbind the frame buffer
            Unbind ();
        }

        /// <summary>
        /// Clear the frame buffer.
        /// </summary>
        public void Clear () {
            GL.Clear (ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }

        /// <summary>
        /// Bind the frame buffer.
        /// </summary>
        public void Bind () {
            Bind (this);
        }

        /// <summary>
        /// Unbind the frame buffer.
        /// </summary>
        public void Unbind () {
            Unbind (this);
        }

        /// <summary>
        /// Bind the specified frame buffer.
        /// </summary>
        /// <param name="framebuffer">Frame buffer.</param>
        public static void Bind (FrameBuffer framebuffer) {

            // Bind the frame buffer
            GL.BindFramebuffer (framebuffer.target, framebuffer.bufferId);

            // Set the viewport up
            GL.Viewport (0, 0, framebuffer.Width, framebuffer.Height);
        }

        /// <summary>
        /// Unbind the specified frame buffer.
        /// </summary>
        /// <param name="framebuffer">Frame buffer.</param>
        public static void Unbind (FrameBuffer framebuffer) {

            // Unbind the frame buffer
            GL.BindFramebuffer (framebuffer.target, 0);
        }
    }
}

