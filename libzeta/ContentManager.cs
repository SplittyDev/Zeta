using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace libzeta {

    /// <summary>
    /// Content manager.
    /// </summary>
    public class ContentManager {

        /// <summary>
        /// The content cache.
        /// </summary>
        readonly HashSet<Content> contentCache;

        /// <summary>
        /// The content provider cache.
        /// </summary>
        readonly HashSet<IContentProvider> contentProviderCache;

        /// <summary>
        /// The root directory of the content.
        /// </summary>
        string contentRoot;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:libzeta.ContentManager"/> class.
        /// </summary>
        /// <param name="root">Root.</param>
        public ContentManager (string root = "") {

            // Initialize the caches
            contentCache = new HashSet<Content> ();
            contentProviderCache = new HashSet<IContentProvider> ();

            // Set the content root
            contentRoot = NormalizePath (root);
            if (string.IsNullOrEmpty (root)) {
                contentRoot = NormalizePath ($"{Environment.CurrentDirectory}/content");
            }
        }

        /// <summary>
        /// Loads the asset with the specified name.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <typeparam name="TContent">The 1st type parameter.</typeparam>
        public TContent Load<TContent> (string name, params object [] args) where TContent : class, IContentProvider, new() {

            // Get the path to the requested asset
            var path = NormalizePath ($"{contentRoot}/{name}");

            this.Log (LoggingLevel.INFO, $"Loading asset: {Path.GetFileNameWithoutExtension (path)}");

            // Get the content provider
            TContent contentProvider;
            if (!contentProviderCache.Any (t => t is TContent)) {
                contentProviderCache.Add (contentProvider = new TContent ());
            } else {
                contentProvider = (TContent)contentProviderCache.First (t => t is TContent);
            }

            // Get the content
            var rawContent = (TContent)contentProvider.LoadContent (path, args);

            // TODO: Cache content

            return rawContent;
        }

        /// <summary>
        /// Normalizes a path.
        /// </summary>
        /// <returns>The normalized path.</returns>
        /// <param name="path">The path.</param>
        string NormalizePath (string path) {
            var chr = Path.DirectorySeparatorChar;
            if (path?.Length == 0) {
                return string.Empty;
            }
            if (chr == '/') {
                path = path.Replace ('\\', '/');
            } else if (chr == '\\') {
                path = path.Replace ('/', '\\');
            } else {
                path = path.Replace ('/', chr);
                path = path.Replace ('\\', chr);
            }
            return Path.GetFullPath (path);
        }
    }
}

