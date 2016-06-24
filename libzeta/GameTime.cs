using System;
namespace libzeta {
    
    /// <summary>
    /// Game time.
    /// </summary>
    public struct GameTime : IComparable<GameTime> {

        /// <summary>
        /// The time delta since the last update.
        /// </summary>
        public readonly TimeSpan Delta;

        /// <summary>
        /// The time since the first update.
        /// </summary>
        public readonly TimeSpan Total;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:libzeta.GameTime"/> struct.
        /// </summary>
        /// <param name="delta">Delta.</param>
        /// <param name="total">Total.</param>
        public GameTime (TimeSpan delta, TimeSpan total) : this () {
            Delta = delta;
            Total = total;
        }

        /// <summary>
        /// Compares the game time to another game time.
        /// </summary>
        /// <param name="other">The other game time.</param>
        public int CompareTo (GameTime other) => Total.CompareTo (other.Total);
    }
}

