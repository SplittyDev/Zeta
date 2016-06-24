using System;
using OpenTK.Input;

namespace libzeta {
    
    /// <summary>
    /// Key state.
    /// </summary>
    public class KeyState {

        /// <summary>
        /// The key.
        /// </summary>
        public Key Key;

        /// <summary>
        /// Whether the key is pressed.
        /// </summary>
        public bool IsDown;

        /// <summary>
        /// Whether the key is frozen.
        /// </summary>
        public bool IsFrozen;

        /// <summary>
        /// Creates a new KeyState from the given key.
        /// </summary>
        /// <param name="key">The key.</param>
        public static KeyState FromKey (Key key) {
            return new KeyState {
                Key = key,
                IsDown = false,
                IsFrozen = false,
            };
        }
    }
}

