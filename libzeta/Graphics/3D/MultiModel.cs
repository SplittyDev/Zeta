using System;
using System.Collections;
using System.Collections.Generic;
using OpenTK;

namespace libzeta {

    /// <summary>
    /// Multi model.
    /// </summary>
    public class MultiModel : ICollection<Model>, IContentProvider {

        /// <summary>
        /// The models.
        /// </summary>
        readonly List<Model> models;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:libzeta.MultiModel"/> class.
        /// </summary>
        public MultiModel () {
            models = new List<Model> ();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:libzeta.MultiModel"/> class.
        /// </summary>
        /// <param name="models">Models.</param>
        public MultiModel (ICollection<Model> models) : this () {
            foreach (var model in models) {
                this.models.Add (model);
            }
        }

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>The count.</value>
        public int Count => models.Count;

        /// <summary>
        /// Gets whether the collection is read-only.
        /// </summary>
        /// <value>The is read only.</value>
        public bool IsReadOnly => ((ICollection<Model>)models).IsReadOnly;

        /// <summary>
        /// Sets the positions.
        /// </summary>
        /// <param name="positions">Positions.</param>
        public void SetPositions (params Vector3 [] positions) {
            var limit = Math.Min (positions.Length, models.Count);
            for (var i = 0; i < limit; i++) {
                models [i].Position = positions [i];
            }
        }

        /// <summary>
        /// Adds a model to the collection.
        /// </summary>
        /// <param name="item">Item.</param>
        public void Add (Model item) {
            models.Add (item);
        }

        /// <summary>
        /// Clears the collection.
        /// </summary>
        public void Clear () {
            models.Clear ();
        }

        /// <summary>
        /// Tests if the collection contains the specified model.
        /// </summary>
        /// <param name="item">Item.</param>
        public bool Contains (Model item) {
            return models.Contains (item);
        }

        /// <summary>
        /// Copies the specified models to an array.
        /// </summary>
        /// <param name="array">Array.</param>
        /// <param name="arrayIndex">Array index.</param>
        public void CopyTo (Model [] array, int arrayIndex) {
            models.CopyTo (array, arrayIndex);
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>The enumerator.</returns>
        public IEnumerator<Model> GetEnumerator () {
            return ((ICollection<Model>)models).GetEnumerator ();
        }

        /// <summary>
        /// Removes the specified item.
        /// </summary>
        /// <param name="item">Item.</param>
        public bool Remove (Model item) {
            return models.Remove (item);
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>The enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator () {
            return ((ICollection<Model>)models).GetEnumerator ();
        }

        /// <summary>
        /// Loads the content.
        /// </summary>
        /// <returns>The content.</returns>
        /// <param name="path">Path.</param>
        /// <param name="args">Arguments.</param>
        public object LoadContent (string path, params object [] args) {
            var geometryList = ModelLoader.LoadGeometry (path);
            var modelList = new List<Model> (geometryList.Count);
            foreach (var geometry in geometryList) {
                modelList.Add (new Model (geometry));
            }
            return new MultiModel (modelList);
        }
    }
}

