using System;
namespace libzeta {

    /// <summary>
    /// Game component.
    /// </summary>
    public abstract class GameComponent {

        /// <summary>
        /// The global identifier.
        /// </summary>
        static int globalId;

        /// <summary>
        /// First magic number.
        /// </summary>
        static int magic1 = 0x666AC1D;

        /// <summary>
        /// Second magic number.
        /// </summary>
        static int magic2 = 0xFEEEEED;

        /// <summary>
        /// The identifier.
        /// </summary>
        internal int id;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:libzeta.GameComponent"/> class.
        /// </summary>
        protected GameComponent () {
            id = globalId++;
        }

        /// <summary>
        /// Update the game logic here.
        /// Executes as fast as possible and possibly
        /// more than once during a single frame.
        /// </summary>
        /// <param name="time">Time.</param>
        public virtual void Update (GameTime time) { }

        /// <summary>
        /// Update the game logic here.
        /// Guaranteed to run once per frame.
        /// </summary>
        /// <returns>The update.</returns>
        /// <param name="time">Time.</param>
        public virtual void FixedUpdate (GameTime time) { }

        /// <summary>
        /// Draw the game graphics here.
        /// </summary>
        public virtual void Draw () { }

        /// <summary>
        /// Gets the hash code.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode () {
            return (magic1 | id) ^ magic2;
        }

        /// <summary>
        /// Tests if the specified object is a game component.
        /// </summary>
        /// <param name="o">The object.</param>
        public static bool IsGameComponent (object o) {
            return ((o.GetHashCode () ^ magic2) & magic1) == magic1;
        }
    }
}

