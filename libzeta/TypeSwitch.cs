using System;
namespace libzeta {

    /// <summary>
    /// Type switch.
    /// </summary>
    public static class TypeSwitch {

        public static Switch<TSource> On<TSource> (TSource value) {
            return new Switch<TSource> (value);
        }

        /// <summary>
        /// Switch.
        /// </summary>
        public sealed class Switch<TSource> {

            /// <summary>
            /// The value.
            /// </summary>
            readonly TSource value;

            /// <summary>
            /// Whether the value was handled.
            /// </summary>
            bool handled;

            /// <summary>
            /// Initializes a new instance of the <see cref="T:libzeta.TypeSwitch.Switch`1"/> class.
            /// </summary>
            /// <param name="value">Value.</param>
            internal Switch (TSource value) {
                this.value = value;
            }

            public Switch<TSource> Case<TTarget> (Action<TTarget> action)
                where TTarget : TSource {
                if (!handled && value is TTarget) {
                    action ((TTarget)value);
                    handled = true;
                }
                return this;
            }

            public void Default (Action<TSource> action) {
                if (!handled)
                    action (value);
            }
        }
    }
}

