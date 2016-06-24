using System;
namespace libzeta {

    /// <summary>
    /// Texture configuration.
    /// </summary>
    public class TextureConfiguration {

        /// <summary>
        /// Whether mipmaps should be created.
        /// </summary>
        public bool Mipmap;

        /// <summary>
        /// The interpolation mode.
        /// </summary>
        public InterpolationMode Interpolation;

        /// <summary>
        /// A texture configuration that uses linear interpolation.
        /// </summary>
        public static TextureConfiguration Linear = new TextureConfiguration {
            Mipmap = false,
            Interpolation = InterpolationMode.Linear
        };

        /// <summary>
        /// A texture configuration that uses nearest-neighbor interpolation.
        /// </summary>
        public static TextureConfiguration Nearest = new TextureConfiguration {
            Mipmap = false,
            Interpolation = InterpolationMode.Nearest
        };

        /// <summary>
        /// A texture configuration that uses linear interpolation with mipmapping.
        /// </summary>
        public static TextureConfiguration LinearMipmap = new TextureConfiguration {
            Mipmap = true,
            Interpolation = InterpolationMode.Linear
        };

        /// <summary>
        /// A texture configuration that uses nearest-neighbor interpolation with mipmapping.
        /// </summary>
        public static TextureConfiguration NearestMipmap = new TextureConfiguration {
            Mipmap = true,
            Interpolation = InterpolationMode.Nearest
        };

        /// <summary>
        /// A texture configuration for framebuffers.
        /// </summary>
        public static TextureConfiguration Framebuffer = new TextureConfiguration {
            Mipmap = false,
            Interpolation = InterpolationMode.Linear
        };
    }
}

