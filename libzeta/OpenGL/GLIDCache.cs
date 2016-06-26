using System;
using System.Collections;
using System.Collections.Generic;

namespace libzeta {

    /// <summary>
    /// OpenGL ID cache.
    /// </summary>
    public class GLIDCache : IEnumerable<int> {

        /// <summary>
        /// The sync root.
        /// </summary>
        readonly object syncRoot;

        /// <summary>
        /// The identifiers.
        /// </summary>
        readonly Stack<int> ids;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:libzeta.GLIDCache"/> class.
        /// </summary>
        public GLIDCache () {
            syncRoot = new object ();
            ids = new Stack<int> ();
        }

        /// <summary>
        /// Pushes the specified id.
        /// </summary>
        /// <param name="id">Identifier.</param>
        public void Push (int id) {
            lock (syncRoot) {
                ids.Push (id);
            }
        }

        /// <summary>
        /// Pops the last id.
        /// </summary>
        public int Pop () {
            lock (syncRoot) {
                if (ids.Count == 0)
                    return 0;
                return ids.Pop ();
            }
        }

        public IEnumerator<int> GetEnumerator () {
            return ids.GetEnumerator ();
        }

        IEnumerator IEnumerable.GetEnumerator () {
            return ids.GetEnumerator ();
        }
    }
}

