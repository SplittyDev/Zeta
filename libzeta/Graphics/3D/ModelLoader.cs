using System;
using System.Collections.Generic;
using System.Linq;
using Assimp;
using Assimp.Configs;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace libzeta {

    /// <summary>
    /// Model loader.
    /// </summary>
    public static class ModelLoader {

        /// <summary>
        /// Loads geometry from a file.
        /// </summary>
        /// <returns>The geometry.</returns>
        /// <param name="path">Path.</param>
        public static List<Mesh> LoadGeometry (string path) {

            // Create the assimp context
            var context = new AssimpContext ();

            // Create the smoothing configuration
            var smoothing = new NormalSmoothingAngleConfig (66f);
            context.SetConfig (smoothing);

            // Create a log stream
            var logStream = new LogStream ((msg, userData) => {

                // Trim the message and the user data
                msg = msg.Trim (' ', '\n');
                userData = userData.Trim (' ', '\n');

                // Determine the logging level
                var level = LoggingLevel.INFO;
                if (msg.StartsWith ("error", StringComparison.OrdinalIgnoreCase))
                    level = LoggingLevel.ERROR;

                // Strip the assimp logging level from the message
                msg = msg.Substring (msg.IndexOf (',') + 1).Trim ();

                // Log the message
                Logging.Log (level, $"{msg}");
            });

            // Attach the log stream
            logStream.Attach ();

            // Set post processing flags
            var flags = 0x0
                | PostProcessSteps.CalculateTangentSpace
                | PostProcessSteps.Triangulate
                | PostProcessSteps.GenerateSmoothNormals
                | PostProcessSteps.TransformUVCoords;

            // Import the scene
            var scene = context.ImportFile (path, flags);

            // Create the geometry list
            var geometries = new List<Mesh> (scene.MeshCount);

            // Iterate over the meshes
            foreach (var mesh in scene.Meshes) {

                // Create the position-, texture- and normal-coordinate lists
                var posCoords = new List<Vector3> ();
                var texCoords = new List<Vector2> ();
                var nrmCoords = new List<Vector3> ();

                // Test if the mesh has texture coordinates
                var hasTexture = mesh.HasTextureCoords (0);

                // Iterate over the faces
                foreach (var face in mesh.Faces) {

                    // Iterate over the indices
                    foreach (var index in face.Indices) {

                        // Get the vertex and normal
                        var vertex = mesh.Vertices [index];
                        var normal = mesh.Normals [index];

                        // Add the position coordinates to the list
                        var posCoord = new Vector3 (vertex.X, vertex.Y, vertex.Z);
                        posCoords.Add (posCoord);

                        // Add the normal coordinates to the list
                        var nrmCoord = new Vector3 (normal.X, normal.Y, normal.Z);
                        nrmCoords.Add (nrmCoord);

                        // Test if the mesh has texture coordinates
                        if (hasTexture) {

                            // Get the texture
                            var texture = mesh.TextureCoordinateChannels [0] [index];

                            // Add the texture coordinates to the list
                            var texCoord = new Vector2 (texture.X, texture.Y);
                            texCoords.Add (texCoord);
                        }
                    }
                }

                // Create the geometry
                var geometry = new Mesh (BeginMode.Triangles);

                // Add the position coordinates
                var posBuffer = new GLBuffer<Vector3> (
                    settings: GLBufferSettings.StaticDraw3FloatArray,
                    buffer: posCoords
                );
                geometry.AddBuffer ("v_pos", posBuffer);

                // Test if the mesh has texture coordinates
                if (hasTexture) {

                    // Add the texture coordinates
                    var texBuffer = new GLBuffer<Vector2> (
                        settings: GLBufferSettings.StaticDraw2FloatArray,
                        buffer: texCoords
                    );
                    geometry.AddBuffer ("v_tex", texBuffer);

                    // Add the normal coordinates
                    var nrmBuffer = new GLBuffer<Vector3> (
                        settings: GLBufferSettings.StaticDraw3FloatArray,
                        buffer: nrmCoords
                    );
                    geometry.AddBuffer ("v_nrm", nrmBuffer);
                }

                // Add the geometry to the list
                geometries.Add (geometry);
            }

            var wordPiece = geometries.Count == 1 ? "piece" : "pieces";
            Logging.Log (LoggingLevel.INFO, $"Imported {geometries.Count} {wordPiece} of geometry.");

            // Return the geometry list
            return geometries;
        }
    }
}

