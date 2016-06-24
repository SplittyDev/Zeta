using System;
using System.Runtime.CompilerServices;

namespace libzeta {

    /// <summary>
    /// Randomizer.
    /// </summary>
    public static class Randomizer {

        /// <summary>
        /// The random number generator.
        /// </summary>
        [ThreadStatic]
        static Random random;

        // State
        static uint x, y, z, w;

        /// <summary>
        /// Initializes the <see cref="T:libzeta.Randomizer"/> class.
        /// </summary>
        static Randomizer () {
            random = new Random ();
            x = (uint)(uint.MaxValue * random.NextDouble ());
            y = (uint)(uint.MaxValue * random.NextDouble ());
            z = (uint)(uint.MaxValue * random.NextDouble ());
            w = (uint)(uint.MaxValue * random.NextDouble ());
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        static uint Next () {
            uint t = x;
            t ^= t << 11;
            t ^= t >> 8;
            x = y;
            y = z;
            z = w;
            w ^= w >> 19;
            w ^= t;
            return w;
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static float NextFloat () {
            return (float)Next () / uint.MaxValue;
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static uint Next (uint min, uint max) {
            return Clamp (min + NextFloat () * max, min, max);
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static int Next (int min, int max) {
            return (int)Next ((uint)min, (uint)max);
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static float Next (float min, float max) {
            return MathF.Clamp (min + NextFloat () * max, min + float.Epsilon, max - float.Epsilon);
        }

        /// <summary>
        /// Randomly chooses one of the specified items.
        /// </summary>
        /// <param name="items">Items.</param>
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static object Choose (params object [] items) {
            return items [Next (0, items.Length)];
        }

        /// <summary>
        /// Randomly returns one of the specified items.
        /// </summary>
        /// <param name="items">Items.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static T Choose<T> (params T [] items) {
            return items [Next (0, items.Length)];
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        static uint Clamp (float val, uint min, uint max) {
            var intval = (uint)val;
            return intval < min ? min : intval > max ? max : intval;
        }
    }
}

