using System;
namespace libzeta {

    /// <summary>
    /// Content.
    /// </summary>
    public abstract class Content {

        /// <summary>
        /// The global identifier.
        /// </summary>
        static int globalId;

        /// <summary>
        /// First magic number.
        /// </summary>
        static int magic1 = 0x13BCA13;

        /// <summary>
        /// Second magic number.
        /// </summary>
        static int magic2 = 0xFFACDCC;

        /// <summary>
        /// The identifier.
        /// </summary>
        internal int id;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:libzeta.Content"/> class.
        /// </summary>
        protected Content () {
            id = ++globalId;
        }

        /// <summary>
        /// Gets the hash code.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode () {
            return (magic1 | id) ^ magic2;
        }

        /// <summary>
        /// Tests if the specified object is content.
        /// </summary>
        /// <param name="o">The object.</param>
        public static bool IsContent (object o) {
            return ((o.GetHashCode () ^ magic2) & magic1) == magic1;
        }
    }

    /// <summary>
    /// Content.
    /// </summary>
    public class Content<T> : Content where T : IContentProvider {

        /// <summary>
        /// The object.
        /// </summary>
        internal readonly T contentObject;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:libzeta.Content`1"/> class.
        /// </summary>
        /// <param name="content">Content.</param>
        public Content (T content) {
            contentObject = content;
        }

        public static implicit operator T (Content<T> content) => content.contentObject;
    }
}

