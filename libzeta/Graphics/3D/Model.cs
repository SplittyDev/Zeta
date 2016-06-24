using System;
using OpenTK;

namespace libzeta {
    
    /// <summary>
    /// Model.
    /// </summary>
    public class Model : IContentProvider {

        /// <summary>
        /// The geometry.
        /// </summary>
        internal Mesh geometry;

        /// <summary>
        /// The position.
        /// </summary>
        public Vector3 Position;

        /// <summary>
        /// The rotation.
        /// </summary>
        public Vector3 Rotation;

        /// <summary>
        /// The scale.
        /// </summary>
        public Vector3 Scale;

        /// <summary>
        /// Gets the material.
        /// </summary>
        /// <value>The material.</value>
        public Material Material => geometry.Material;

        /// <summary>
        /// Gets the matrix.
        /// </summary>
        /// <value>The matrix.</value>
        public Matrix4 ModelMatrix {
            get {

                // Create scale matrix
                var scale = Matrix4.CreateScale (Scale);

                // Create rotation matrix for x axis
                var rotx = Matrix4.CreateRotationX (Rotation.X);

                // Create rotation matrix for y axis
                var roty = Matrix4.CreateRotationY (Rotation.Y);

                // Create rotation matrix for z axis
                var rotz = Matrix4.CreateRotationZ (Rotation.Z);

                // Create translation matrix
                var translation = Matrix4.CreateTranslation (Position);

                // Multiply all matrices together
                return scale * rotx * roty * rotz * translation;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:libzeta.Model"/> class.
        /// </summary>
        public Model () {
            Scale = Vector3.One;
            Position = Vector3.Zero;
            Rotation = Vector3.Zero;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:libzeta.Model"/> class.
        /// </summary>
        /// <param name="geometry">Geometry.</param>
        public Model (Mesh geometry) : this () {
            this.geometry = geometry;
        }

        /// <summary>
        /// Draw the model.
        /// </summary>
        /// <param name="program">Program.</param>
        /// <param name="camera">Camera.</param>
        public void Draw (ShaderProgram program, Camera camera) {

            // Draw the geometry
            geometry.Draw (program, ModelMatrix, camera);
        }

        /// <summary>
        /// Draw the model.
        /// </summary>
        /// <param name="program">Program.</param>
        /// <param name="viewProjection">View projection.</param>
        public void Draw (ShaderProgram program, Matrix4 viewProjection) {

            // Draw the geometry
            geometry.Draw (program, ModelMatrix, viewProjection);
        }

        /// <summary>
        /// Loads the content.
        /// </summary>
        /// <returns>The content.</returns>
        /// <param name="path">Path.</param>
        /// <param name="args">Arguments.</param>
        public object LoadContent (string path, params object [] args) {

            // Check the first argument
            if (args.Length == 1 && !(args [0] is int)) {
                throw new ArgumentException ("The first argument is expected to be of type int.", nameof (args));
            }

            // Load the geometry
            var geometryIndex = args.Length == 1 ? (int)args [0] : 0;
            var loadedGeometry = Mesh.LoadFrom (path, geometryIndex);

            // Create the model
            return new Model (loadedGeometry);
        }

        /*
        public Model (ObjFile objModel, int groupNum, ShaderProgram program) {
            var group = objModel.Groups [groupNum];
            var tempPos = new List<Vector3> ();
            var tempTex = new List<Vector2> ();
            foreach (ObjFace f in group.Faces) {
                foreach (ObjFaceVertex vert in f.Vertices) {
                    tempPos.Add (objModel.Vertices [vert.VertexIndex - 1]);
                    tempTex.Add (objModel.Textures [vert.TextureIndex - 1]);
                }
            }
            var v_pos = new GLBuffer<Vector3> (GLBufferSettings.StaticDraw3FloatArray, tempPos);
            var v_tex = new GLBuffer<Vector2> (GLBufferSettings.StaticDraw2FloatArray, tempTex);
            var m_ind = new GLBuffer<uint> (GLBufferSettings.StaticIndices, Array.ConvertAll<int, uint> (Enumerable.Range (0, tempPos.Count).ToArray (), x => (uint)x));
            Geometry = new Geometry (BeginMode.Quads)
                .AddBuffer ("v_pos", v_pos)
                .AddBuffer ("v_tex", v_tex);

            Position = Vector3.Zero;
            Scale = Vector3.One;
            Rotation = Vector3.Zero;
        }
        */
    }
}

