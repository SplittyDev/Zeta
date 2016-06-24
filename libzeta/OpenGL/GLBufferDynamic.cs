using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;

namespace libzeta {

    /// <summary>
    /// Dynamic OpenGL buffer.
    /// </summary>
    public class GLBufferDynamic<T> : IBuffer<int> where T : struct {

        /// <summary>
        /// The buffer identifier.
        /// </summary>
        readonly public int BufferId;

        /// <summary>
        /// The settings.
        /// </summary>
        public GLBufferSettings Settings;

        /// <summary>
        /// Gets or sets the size of the buffer.
        /// </summary>
        /// <value>The size of the buffer.</value>
        public int BufferSize { get; set; }

        /// <summary>
        /// Gets the size of the element.
        /// </summary>
        /// <value>The size of the element.</value>
        public int ElementSize { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:libzeta.GLBufferDynamic`1"/> class.
        /// </summary>
        /// <param name="settings">Settings.</param>
        /// <param name="elementSize">Element size.</param>
        /// <param name="startCapacity">Start capacity.</param>
        public GLBufferDynamic (GLBufferSettings settings, int elementSize, int startCapacity = 8192) {
            Settings = settings;
            BufferSize = startCapacity;
            ElementSize = elementSize;

            BufferId = GL.GenBuffer ();
            Bind ();
            GL.BufferData (Settings.Target, BufferSize, IntPtr.Zero, Settings.Hint);
            Unbind ();
        }

        /// <summary>
        /// Uploads the data.
        /// </summary>
        /// <returns>The data.</returns>
        /// <param name="dataArray">Data array.</param>
        public void UploadData (T [] dataArray) {

            // Check if data is null
            if (dataArray == null)
                throw new ArgumentException ("Cannot upload data: data is null", nameof (dataArray));

            // Calculate the buffer size
            var bufferSize = Marshal.SizeOf (dataArray [0]) * dataArray.Length;

            Bind ();
            GL.BufferData (Settings.Target, bufferSize, dataArray, Settings.Hint);
            Unbind ();
        }

        /// <summary>
        /// Uploads the data.
        /// </summary>
        /// <returns>The data.</returns>
        /// <param name="dataList">Data list.</param>
        public void UploadData (IList<T> dataList) {

            // Check if data is null
            if (dataList == null)
                throw new ArgumentException ("Cannot upload data: data is null", nameof (dataList));

            // Calculate the buffer size
            var bufferSize = Marshal.SizeOf (dataList [0]) * dataList.Count;

            Bind ();
            GL.BufferData (Settings.Target, bufferSize, dataList.ToArray (), Settings.Hint);
            Unbind ();
        }

        /// <summary>
        /// Bind the specified buffer.
        /// </summary>
        /// <param name="buffer">Buffer.</param>
        /// <typeparam name="BuffType">The 1st type parameter.</typeparam>
        public static void Bind<BuffType> (GLBufferDynamic<BuffType> buffer) where BuffType : struct {

            // Check if buffer is null
            if (buffer == null)
                throw new ArgumentException ("Cannot bind buffer: buffer is null", nameof (buffer));

            GL.BindBuffer (buffer.Settings.Target, buffer.BufferId);
        }

        /// <summary>
        /// Unbind the specified buffer.
        /// </summary>
        /// <param name="buffer">Buffer.</param>
        /// <typeparam name="BuffType">The 1st type parameter.</typeparam>
        public static void Unbind<BuffType> (GLBufferDynamic<BuffType> buffer) where BuffType : struct {

            // Check if buffer is null
            if (buffer == null)
                throw new ArgumentException ("Cannot bind buffer: buffer is null", nameof (buffer));

            GL.BindBuffer (buffer.Settings.Target, 0);
        }

        /// <summary>
        /// Bind this instance.
        /// </summary>
        public void Bind () {
            Bind (this);
        }

        /// <summary>
        /// Unbind this instance.
        /// </summary>
        public void Unbind () {
            Unbind (this);
        }

        /// <summary>
        /// Point to the specified vertex.
        /// </summary>
        /// <param name="where">Where.</param>
        public void PointTo (int where) {

            // Bind buffer
            Bind ();

            // Enable vertex attribute array
            GL.EnableVertexAttribArray (where);

            // Set vertex attribute pointer
            GL.VertexAttribPointer (where, Settings.AttribSize, Settings.Type, Settings.Normalized, ElementSize, Settings.Offset);

            // Unbind buffer
            Unbind ();
        }

        /// <summary>
        /// Point to the specified vertex.
        /// </summary>
        /// <param name="where">Where.</param>
        /// <param name="offset">Offset.</param>
        public void PointTo (int where, int offset) {

            // Bind buffer
            Bind ();

            // Enable vertex attribute array
            GL.EnableVertexAttribArray (where);

            // Set vertex attribute pointer
            GL.VertexAttribPointer (where, Settings.AttribSize, Settings.Type, Settings.Normalized, ElementSize, offset);

            // Unbind buffer
            Unbind ();
        }

        /// <summary>
        /// Point to the specified vertices.
        /// </summary>
        /// <param name="where">Where.</param>
        /// <param name="other">Other.</param>
        public void PointTo (int where, params int [] other) {

            // Check if the other parameter is null
            if (other == null)
                throw new ArgumentException ("Cannot set attribute pointer: other is null", nameof (other));

            // Check if other contains at least two elements
            if (other.Length < 2)
                throw new ArgumentException ("Cannot set attribute pointer: other contains less than two elements", nameof (other));

            // Bind buffer
            Bind ();

            // Enable vertex attribute array
            GL.EnableVertexAttribArray (where);

            // Set vertex attribute pointer
            GL.VertexAttribPointer (where, other [0], Settings.Type, Settings.Normalized, ElementSize, other [1]);

            // Unbind buffer
            Unbind ();
        }
    }
}

