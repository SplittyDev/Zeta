using System;
using System.Runtime.CompilerServices;
using OpenTK;

namespace libzeta {

    /// <summary>
    /// Floating point math.
    /// </summary>
    public static class MathF {

        /// <summary>
        /// The E constant.
        /// </summary>
        public const float E = 2.71828182845905f;

        /// <summary>
        /// The PI constant.
        /// </summary>
        public const float PI = 3.14159265358979f;

        /// <summary>
        /// The PHI constant.
        /// </summary>
        public const float PHI = 1.61803398874989f;

        /// <summary>
        /// An approximation of the square root of 2.
        /// </summary>
        public const float SQRT2 = 1.41421356237309f;

        /// <summary>
        /// Linearly interpolates between start and end by t.
        /// </summary>
        /// <param name="start">Start.</param>
        /// <param name="end">End.</param>
        /// <param name="t">Time.</param>
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static float Lerp (float start, float end, float t) {
            return start + Clamp (t, 0f, 1f) * (end - start);
        }

        /// <summary>
        /// Linearly interpolates between start and end by t.
        /// </summary>
        /// <param name="start">Start.</param>
        /// <param name="end">End.</param>
        /// <param name="t">Time.</param>
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static Vector2 Lerp (Vector2 start, Vector2 end, float t) {
            return start + Clamp (t, 0f, 1f) * (end - start);
        }

        /// <summary>
        /// Linearly interpolates between start and end by t.
        /// </summary>
        /// <param name="start">Start.</param>
        /// <param name="end">End.</param>
        /// <param name="t">Time.</param>
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static Vector3 Lerp (Vector3 start, Vector3 end, float t) {
            return start + Clamp (t, 0f, 1f) * (end - start);
        }

        /// <summary>
        /// Linearly interpolates between start and end by t.
        /// </summary>
        /// <param name="start">Start.</param>
        /// <param name="end">End.</param>
        /// <param name="t">Time.</param>
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static Vector4 Lerp (Vector4 start, Vector4 end, float t) {
            return start + Clamp (t, 0f, 1f) * (end - start);
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static Vector2 Slerp (Vector2 start, Vector2 end, float t) {
            var dot = Clamp (Vector2.Dot (start, end), -1f, 1f);
            var theta = Acos (dot) * Clamp (t, 0f, 1f);
            return start * Cos (theta) + (end - start * dot).Normalized () * Sin (theta);
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static Vector3 Slerp (Vector3 start, Vector3 end, float t) {
            var dot = Clamp (Vector3.Dot (start, end), -1f, 1f);
            var theta = Acos (dot) * Clamp (t, 0f, 1f);
            return start * Cos (theta) + (end - start * dot).Normalized () * Sin (theta);
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static Vector4 Slerp (Vector4 start, Vector4 end, float t) {
            var dot = Clamp (Vector4.Dot (start, end), -1f, 1f);
            var theta = Acos (dot) * Clamp (t, 0f, 1f);
            return start * Cos (theta) + (end - start * dot).Normalized () * Sin (theta);
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static Vector2 Nlerp (Vector2 start, Vector2 end, float t) {
            return Lerp (start, end, t).Normalized ();
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static Vector3 Nlerp (Vector3 start, Vector3 end, float t) {
            return Lerp (start, end, t).Normalized ();
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static Vector4 Nlerp (Vector4 start, Vector4 end, float t) {
            return Lerp (start, end, t).Normalized ();
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static float Clamp (float val, float min, float max) {
            return val < min ? min : val > max ? max : val;
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static Vector2 Clamp (Vector2 vec, float min, float max) {
            return new Vector2 (
                x: Clamp (vec.X, min, max),
                y: Clamp (vec.Y, min, max)
            );
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static Vector3 Clamp (Vector3 vec, float min, float max) {
            return new Vector3 (
                x: Clamp (vec.X, min, max),
                y: Clamp (vec.Y, min, max),
                z: Clamp (vec.Z, min, max)
            );
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static Vector4 Clamp (Vector4 vec, float min, float max) {
            return new Vector4 (
                x: Clamp (vec.X, min, max),
                y: Clamp (vec.Y, min, max),
                z: Clamp (vec.Z, min, max),
                w: Clamp (vec.W, min, max)
            );
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static float Sin (float deg) {
            return (float)Math.Sin (deg);
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static float Min (float a, float b) {
            return a < b ? a : b;
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static float Max (float a, float b) {
            return a > b ? a : b;
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static float Sinh (float d) {
            return (float)Math.Sinh (d);
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static float Asin (float d) {
            return (float)Math.Asin (d);
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static float Cos (float d) {
            return (float)Math.Cos (d);
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static float Cosh (float d) {
            return (float)Math.Cosh (d);
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static float Acos (float d) {
            return (float)Math.Acos (d);
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static float Tan (float d) {
            return (float)Math.Tan (d);
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static float Atan (float d) {
            return (float)Math.Atan (d);
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static float Atan2 (float a, float b) {
            return (float)Math.Atan2 (a, b);
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static float Tanh (float d) {
            return (float)Math.Tanh (d);
        }

        /*
        public static float Sin (float deg, int preciseDigits = 20) {
            if (Math.Abs (deg) < float.Epsilon)
                return 0f;
            deg = deg - (deg * (1f / (2f * PI))) * 2f * PI;
            deg = Min (deg, PI - deg);
            deg = Max (deg, -PI - deg);
            deg = Min (deg, PI - deg);
            var sum = 1f;
            var deg2 = deg * deg;
            for (var n = preciseDigits - 1; n >= 0; n -= 1) {
                var n2 = 2f * n;
                sum = 1f - deg2 / (n2 + 2f) / (n2 + 3f) * sum;
            }
            return deg * sum;
        }
        
        public static float Cos (float deg, int preciseDigits = 20) {
            if (Math.Abs (deg) < float.Epsilon)
                return 1f;
            deg = deg - (deg * (1f / (2f * PI))) * 2f * PI;
            deg = Min (deg, PI - deg);
            deg = Max (deg, -PI - deg);
            deg = Min (deg, PI - deg);
            var sum = 1f;
            var deg2 = deg * deg;
            for (var n = preciseDigits - 1; n >= 0; n -= 1) {
                var n2 = 2f * n;
                sum = 1f - deg2 / (n2 + 2f) / (n2 + 1f) * sum;
            }
            return sum;
        }
        */
    }
}

