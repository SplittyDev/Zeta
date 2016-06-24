using System;
namespace libzeta {

    /// <summary>
    /// Lifetime modifier mode.
    /// </summary>
    public enum LifetimeModifierMode {

        /// <summary>
        /// Randomly multiply the lifetime by a value between [min] and [max].
        /// </summary>
        RandomMultiply,

        /// <summary>
        /// Randomly add [min] to [max] seconds of lifetime.
        /// </summary>
        RandomAdd,
    }
}

