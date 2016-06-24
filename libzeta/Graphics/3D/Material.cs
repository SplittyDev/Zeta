using System;
using OpenTK.Graphics;

namespace libzeta {

    /// <summary>
    /// Material.
    /// </summary>
    public struct Material {

        /// <summary>
        /// The default material.
        /// </summary>
        public readonly static Material Default = new Material (Color4.Gray, Texture2D.Dot);

        /// <summary>
        /// The color.
        /// </summary>
        public Color4 Color;

        /// <summary>
        /// The texture.
        /// </summary>
        public Texture2D Texture;

        /// <summary>
        /// The specular intensity.
        /// </summary>
        public float SpecularIntensity;

        /// <summary>
        /// The specular power.
        /// </summary>
        public float SpecularPower;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:libzeta.Material"/> struct.
        /// </summary>
        /// <param name="color">Color.</param>
        /// <param name="texture">Texture.</param>
        /// <param name="specularIntensity">Specular intensity.</param>
        /// <param name="specularPower">Specular power.</param>
        public Material (Color4 color, Texture2D texture = null, float specularIntensity = 1f, float specularPower = 2f) : this () {
            Color = color;
            Texture = texture ?? Texture2D.Dot;
            SpecularIntensity = specularIntensity;
            SpecularPower = specularPower;
        }
    }
}

