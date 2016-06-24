using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace libzeta {
    
    /// <summary>
    /// Mesh.
    /// </summary>
    public class Mesh : IBindable, IContentProvider {

        /// <summary>
        /// The buffers.
        /// </summary>
        public Dictionary<string, IBuffer<int>> Buffers;

        /// <summary>
        /// The indices.
        /// </summary>
        public GLBuffer<uint> Indices;

        /// <summary>
        /// The material.
        /// </summary>
        public Material Material;

        /// <summary>
        /// The array buffer object.
        /// </summary>
        int abo = -1;

        /// <summary>
        /// The begin mode.
        /// </summary>
        readonly BeginMode beginMode;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:libzeta.Geometry"/> class.
        /// </summary>
        public Mesh (BeginMode mode) {

            // Initialize the material
            Material = Material.Default;

            // Initialize the buffer list
            Buffers = new Dictionary<string, IBuffer<int>> ();

            // Set the begin mode
            beginMode = mode;

            // Create the array buffer object
            abo = GL.GenVertexArray ();
        }

        /// <summary>
        /// Add a buffer to the geometry object.
        /// </summary>
        /// <returns>The buffer.</returns>
        /// <param name="name">Name.</param>
        /// <param name="buffer">Buffer.</param>
        public Mesh AddBuffer (string name, IBuffer<int> buffer) {
            Buffers [name] = buffer;
            return this;
        }

        /// <summary>
        /// Point the buffers to the specified attribute.
        /// </summary>
        /// <param name="program">Program.</param>
        /// <param name="name">Name.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public Mesh Attribute<T> (ShaderProgram program, string name) where T : struct {
            ((GLBuffer<T>)Buffers [name]).PointTo (program.Attrib (name));
            return this;
        }

        /// <summary>
        /// Set the indices.
        /// </summary>
        /// <returns>The indices.</returns>
        /// <param name="indices">Indices.</param>
        public Mesh SetIndices (GLBuffer<uint> indices) {
            Indices = indices;
            return this;
        }

        /// <summary>
        /// Bind the vertices and indices.
        /// </summary>
        /// <param name="this">This.</param>
        public static void Bind (Mesh @this) {
            GL.BindVertexArray (@this.abo);
            @this.Indices?.Bind ();
            foreach (var kvp in @this.Buffers) {
                kvp.Value.Bind ();
            }
        }

        /// <summary>
        /// Bind the vertices and indices.
        /// </summary>
        public void Bind () {
            Bind (this);
        }

        /// <summary>
        /// Unbind the vertices and indices.
        /// </summary>
        /// <param name="this">This.</param>
        public static void Unbind (Mesh @this) {
            GL.BindVertexArray (0);
            @this.Indices?.Unbind ();
            foreach (var kvp in @this.Buffers) {
                kvp.Value.Unbind ();
            }
        }

        /// <summary>
        /// Unbind the vertices and indices.
        /// </summary>
        public void Unbind () {
            Unbind (this);
        }

        /// <summary>
        /// Draw the geometry object.
        /// </summary>
        /// <param name="program">Shader program.</param>
        /// <param name="Model">Model matrix.</param>
        /// <param name="camera">Camera.</param>
        /// <param name="offset">Offset.</param>
        public void Draw (ShaderProgram program, Matrix4 Model, Camera camera, int offset = 0) {
            Draw (program, Model, camera.ViewProjection, offset);
        }

        /// <summary>
        /// Draw the geometry object.
        /// </summary>
        /// <param name="program">Shader program.</param>
        /// <param name="model">Model matrix.</param>
        /// <param name="viewProjection">View projection.</param>
        /// <param name="offset">Offset.</param>
        public void Draw (ShaderProgram program, Matrix4 model, Matrix4 viewProjection, int offset = 0) {

            // Bind the geometry
            Bind ();

            // Set the vertex attributes
            foreach (var kvp in Buffers) {
                kvp.Value.PointTo (program.Attrib (kvp.Key));
            }

            // Set the model view projection
            program ["MVP"] = model * viewProjection;

            // Set the model matrix
            var modelMatrix = model.Inverted ();
            modelMatrix.Transpose ();
            program ["NRM"] = modelMatrix;

            // Set the material uniforms
            program ["material.color"] = Material.Color;
            program ["material.diffuse"] = 0;
            program ["material.specularIntensity"] = Material.SpecularIntensity;
            program ["material.specularPower"] = Material.SpecularPower;

            // Bind the texture of the material
            Material.Texture.Bind ();

            // Draw the geometry
            if (Indices != null) {
                GL.DrawElements (beginMode, Indices.Buffer.Count, DrawElementsType.UnsignedInt, offset);
            } else {
                GL.DrawArrays (beginMode == BeginMode.Triangles ? PrimitiveType.Triangles : PrimitiveType.Quads, 0, Buffers ["v_pos"].BufferSize);
            }

            // Unbind the geometry
            Unbind ();
        }

        /// <summary>
        /// Loads the content.
        /// </summary>
        /// <returns>The content.</returns>
        /// <param name="path">Path.</param>
        /// <param name="args">Arguments.</param>
        public object LoadContent (string path, params object [] args) {
            return LoadFrom (path, args.Length == 1 ? (int) args [0] : 0);
        }

        /// <summary>
        /// Load a specific piece of geometry from a file.
        /// </summary>
        /// <returns>The from.</returns>
        /// <param name="path">Path.</param>
        /// <param name="modelIndex">Model index.</param>
        public static Mesh LoadFrom (string path, int modelIndex) {
            return LoadAllFrom (path) [modelIndex];
        }

        /// <summary>
        /// Load all pieces of geometry from a file.
        /// </summary>
        /// <returns>The all from.</returns>
        /// <param name="path">Path.</param>
        public static List<Mesh> LoadAllFrom (string path) {
            return ModelLoader.LoadGeometry (path);
        }
    }
}

