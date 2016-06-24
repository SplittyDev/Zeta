using System;
using System.IO;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace libzeta {

    /// <summary>
    /// 2D texture.
    /// </summary>
    public class Texture2D : IContentProvider {

        /// <summary>
        /// A fully transparent 1x1 texture.
        /// </summary>
        internal static Texture2D Transparent;

        /// <summary>
        /// A fully opaque 1x1 texture.
        /// </summary>
        internal static Texture2D Dot;

        /// <summary>
        /// Initializes the <see cref="T:libzeta.Texture2D"/> class.
        /// </summary>
        static Texture2D () {
            Color4 [] data;
            Transparent = new Texture2D (
                config: TextureConfiguration.Nearest,
                width: 1,
                height: 1
            );
            data = new [] { Color4.Transparent };
            Transparent.SetData (data);
            Dot = new Texture2D (
                config: TextureConfiguration.Nearest,
                width: 1,
                height: 1
            );
            data = new [] { Color4.White };
            Dot.SetData (data);
        }

        /// <summary>
        /// The texture identifier.
        /// </summary>
        readonly public int TextureId;

        /// <summary>
        /// Gets the width.
        /// </summary>
        /// <value>The width.</value>
        public int Width { get; private set; }

        /// <summary>
        /// Gets the height.
        /// </summary>
        /// <value>The height.</value>
        public int Height { get; private set; }

        /// <summary>
        /// Gets the bounds.
        /// </summary>
        /// <value>The bounds.</value>
        public Rectangle Bounds => new Rectangle (0, 0, Width, Height);

        /// <summary>
        /// Initializes a new instance of the <see cref="T:libzeta.Texture2D"/> class.
        /// </summary>
        public Texture2D () { }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:libzeta.Texture2D"/> class.
        /// </summary>
        /// <param name="target">Target.</param>
        /// <param name="internalFormat">Internal format.</param>
        /// <param name="format">Format.</param>
        /// <param name="type">Type.</param>
        /// <param name="mode">Mode.</param>
        /// <param name="mipmap">Mipmap.</param>
        /// <param name="width">Width.</param>
        /// <param name="height">Height.</param>
        public Texture2D (
            TextureTarget target,
            PixelInternalFormat internalFormat,
            PixelFormat format,
            PixelType type,
            InterpolationMode mode,
            bool mipmap,
            int width,
            int height) {

            // Set the size of the texture
            Width = width;
            Height = height;

            // Get the texture id
            TextureId = GL.GenTexture ();

            // Bind the texture
            Bind (TextureUnit.Texture0);

            // Configure the minifier and magnifier filters
            TextureMinFilter minfilter = TextureMinFilter.Linear;
            TextureMagFilter magfilter = TextureMagFilter.Linear;
            switch (mode) {
            case InterpolationMode.Linear:
                minfilter = mipmap
                    ? TextureMinFilter.LinearMipmapLinear
                    : TextureMinFilter.Linear;
                magfilter = TextureMagFilter.Linear;
                break;
            case InterpolationMode.Nearest:
                minfilter = mipmap
                    ? TextureMinFilter.LinearMipmapNearest
                    : TextureMinFilter.Nearest;
                magfilter = TextureMagFilter.Nearest;
                break;
            }

            // Set the minifier filter parameter
            GL.TexParameter (
                target: TextureTarget.Texture2D,
                pname: TextureParameterName.TextureMinFilter,
                param: (int)minfilter
            );

            // Set the magnifier filter parameter
            GL.TexParameter (
                target: TextureTarget.Texture2D,
                pname: TextureParameterName.TextureMagFilter,
                param: (int)magfilter
            );

            // Create the texture
            GL.TexImage2D (
                target: target,
                level: 0,
                internalformat: internalFormat,
                width: Width,
                height: Height,
                border: 0,
                format: format,
                type: type,
                pixels: IntPtr.Zero
            );

            // Create a mipmap if requested
            if (mipmap) {
                GL.GenerateMipmap (GenerateMipmapTarget.Texture2D);
            }

            // Unbind the texture
            Unbind (TextureUnit.Texture0);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:libzeta.Texture2D"/> class.
        /// </summary>
        /// <param name="config">Config.</param>
        /// <param name="width">Width.</param>
        /// <param name="height">Height.</param>
        public Texture2D (TextureConfiguration config, int width, int height) : this (
            target: TextureTarget.Texture2D,
            internalFormat: PixelInternalFormat.Rgba,
            format: PixelFormat.Bgra,
            type: PixelType.UnsignedByte,
            mode: config.Interpolation,
            mipmap: config.Mipmap, 
            width: width,
            height: height) {
        }

        /// <summary>
        /// Sets the data.
        /// </summary>
        /// <returns>The data.</returns>
        /// <param name="data">Data.</param>
        /// <param name="rect">Rect.</param>
        /// <param name="pixelFormat">Pixel format.</param>
        /// <param name="pixelType">Pixel type.</param>
        public Texture2D SetData (
            IntPtr data,
            Rectangle? rect = null,
            PixelFormat pixelFormat = PixelFormat.Rgba,
            PixelType pixelType = PixelType.UnsignedByte) {

            // Get or create the bounds of the texture data
            Rectangle r = rect ?? new Rectangle (0, 0, Width, Height);

            // Bind the texture
            Bind (TextureUnit.Texture0);

            // Update the texture data
            GL.TexSubImage2D (
                target: TextureTarget.Texture2D,
                level: 0,
                xoffset: 0,
                yoffset: 0,
                width: r.Width,
                height: r.Height,
                format: pixelFormat,
                type: pixelType,
                pixels: data
            );

            // Unbind the texture
            Unbind (TextureUnit.Texture0);
            return this;
        }

        /// <summary>
        /// Sets the data.
        /// </summary>
        /// <returns>The data.</returns>
        /// <param name="data">Data.</param>
        /// <param name="rect">Rect.</param>
        /// <param name="pixelFormat">Pixel format.</param>
        /// <param name="pixelType">Pixel type.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public Texture2D SetData<T> (
            T [] data,
            Rectangle? rect = null,
            PixelFormat pixelFormat = PixelFormat.Bgra,
            PixelType pixelType = PixelType.UnsignedInt)
            where T : struct {

            // Get or create the bounds of the texture data
            Rectangle r = rect ?? new Rectangle (0, 0, Width, Height);

            // Bind the texture
            Bind (TextureUnit.Texture0);

            // Update the texture data
            GL.TexSubImage2D (
                target: TextureTarget.Texture2D,
                level: 0,
                xoffset: r.X,
                yoffset: r.Y,
                width: r.Width,
                height: r.Height,
                format: pixelFormat,
                type: pixelType,
                pixels: data
            );

            // Unbind the texture
            Unbind (TextureUnit.Texture0);
            return this;
        }

        /// <summary>
        /// Bind the texture.
        /// </summary>
        public void Bind () {

            // Bind the texture to texture unit 0
            Bind (TextureUnit.Texture0);
        }

        /// <summary>
        /// Bind the texture.
        /// </summary>
        public void Bind (TextureUnit unit) {

            // Make the texture unit the active one
            GL.ActiveTexture (unit);

            // Bind the texture
            GL.BindTexture (TextureTarget.Texture2D, TextureId);
        }

        /// <summary>
        /// Unbind the texture.
        /// </summary>
        public void Unbind () {

            // Unbind the texture value from texture unit 0
            Unbind (TextureUnit.Texture0);
        }

        /// <summary>
        /// Unbind the texture.
        /// </summary>
        public void Unbind (TextureUnit unit) {

            // Make the texture unit the active one
            GL.ActiveTexture (unit);

            // Unbind the texture
            GL.BindTexture (TextureTarget.Texture2D, 0);
        }

        /// <summary>
        /// Loads the content.
        /// </summary>
        /// <returns>The content.</returns>
        /// <param name="path">Path.</param>
        public object LoadContent (string path, params object [] args) {

            // Throw an exception if the file does not exist
            if (!File.Exists (path)) {
                throw new FileNotFoundException ($"File not found: {path}");
            }

            // Load the image using StbImage
            int x = -1, y = -1, n = -1;
            var data = StbImage.LoadImage (
                filename: path,
                x: ref x,
                y: ref y,
                n: ref n,
                req_comp: 4
            );

            // Get or create the texture configuration
            var config = TextureConfiguration.LinearMipmap;
            if (args.Length == 1) {
                var tmp = args [0] as TextureConfiguration;
                if (tmp == null) {
                    throw new ArgumentException ("Expected a texture configuration as first argument.");
                }
                config = tmp;
            }

            // Create the texture
            var texture = new Texture2D (config, x, y);
            texture.SetData (data);

            // Free the image
            StbImage.FreeImage (data);
            return texture;
        }

        public override bool Equals (object obj) {
            var tex = obj as Texture2D;
            return tex != null && tex.TextureId == TextureId;
        }

        public override int GetHashCode () {
            return 0xCC * TextureId;
        }
    }
}

