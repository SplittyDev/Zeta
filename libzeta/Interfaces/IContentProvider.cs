using System;
namespace libzeta {

    /// <summary>
    /// Interface to classes that provide content.
    /// </summary>
    public interface IContentProvider {

        /// <summary>
        /// Load the content from the specified path.
        /// </summary>
        /// <param name="path">Path.</param>
        /// <param name="args">Arguments.</param>
        object LoadContent (string path, params object [] args);
    }
}

