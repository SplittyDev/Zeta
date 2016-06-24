using System;
using System.Collections.Generic;
using System.Text;

namespace libzeta {
    public abstract partial class ShaderBuilder {

        [AttributeUsage (AttributeTargets.Field)]
        protected class GLSLDeclarationType : Attribute {
            public override string ToString () => string.Empty;
        }

        protected class In : GLSLDeclarationType {
            public override string ToString () => "in";
        }

        protected class Out : GLSLDeclarationType {
            public override string ToString () => "out";
        }

        protected class SmoothOut : GLSLDeclarationType {
            public override string ToString () => "smooth out";
        }

        protected class Uniform : GLSLDeclarationType {
            public override string ToString () => "uniform";
        }

        /// <summary>
        /// GLSL base object.
        /// </summary>
        protected class GLSLObject {
            internal string Name;
            internal Struct owningStruct;
            internal virtual string TypeName => string.Empty;
            internal static StringBuilder builder;
            internal static Dictionary<Type, Struct> structCache;
            public readonly static GLSLObject Inherit = new GLSLObject ();
            public readonly static GLSLObject Null = new GLSLObject ();
            public GLSLObject () { Name = string.Empty; }
            public GLSLObject (string name) {
                Name = name;
            }
            public bool IsOwnedByStruct () => owningStruct != null;
            public void SetName (string name) => Name = name;
            public static GLSLObject operator + (GLSLObject a) {
                return new GLSLObject ($"+{a}");
            }
            public static GLSLObject operator - (GLSLObject a) {
                return new GLSLObject ($"-{a}");
            }
            public static GLSLObject operator + (GLSLObject a, GLSLObject b) {
                return new GLSLObject ($"{a} + {b}");
            }
            public static GLSLObject operator - (GLSLObject a, GLSLObject b) {
                return new GLSLObject ($"{a} - {b}");
            }
            public static GLSLObject operator * (GLSLObject a, GLSLObject b) {
                return new GLSLObject ($"{a} * {b}");
            }
            public static GLSLObject operator / (GLSLObject a, GLSLObject b) {
                return new GLSLObject ($"{a} / {b}");
            }
            public static GLSLObject operator & (GLSLObject a, GLSLObject b) {
                return new GLSLObject ($"{a} & {b}");
            }
            public static GLSLObject operator | (GLSLObject a, GLSLObject b) {
                return new GLSLObject ($"{a} | {b}");
            }
            public static GLSLObject operator % (GLSLObject a, GLSLObject b) {
                return new GLSLObject ($"{a} % {b}");
            }
            public override string ToString () {
                if (IsOwnedByStruct ()) {
                    return $"{owningStruct.FieldName}.{Name}";
                }
                return Name;
            }
            public override bool Equals (object obj) => false;
            public override int GetHashCode () => base.GetHashCode ();
        }

        /// <summary>
        /// GLSL initializer object.
        /// </summary>
        protected class GLSLInitializer {
            protected internal readonly string Name;
            protected internal GLSLInitializer (string name) {
                Name = name;
            }
            public T ToType<T> () where T: GLSLObject {
                return (T)this;
            }
            public static explicit operator GLSLObject (GLSLInitializer init) => new GLSLObject (init.Name);
            public static explicit operator Vec2 (GLSLInitializer init) => new Vec2 (init.Name);
            public static explicit operator Vec3 (GLSLInitializer init) => new Vec3 (init.Name);
            public static explicit operator Vec4 (GLSLInitializer init) => new Vec4 (init.Name);
            public static explicit operator Mat4 (GLSLInitializer init) => new Mat4 (init.Name);
            public static explicit operator Sampler2D (GLSLInitializer init) => new Sampler2D (init.Name);
            public static explicit operator Struct (GLSLInitializer init) => new Struct (init.Name);
            public static explicit operator GLFloat (GLSLInitializer init) => new GLFloat (init.Name);
        }

        protected class Struct : GLSLObject {
            internal override string TypeName => Name;
            internal string FieldName = string.Empty;
            public Struct () { }
            public Struct (string name) : base (name) {
                FieldName = name.ToLowerInvariant ();
            
            }
        }

        /// <summary>
        /// Float.
        /// </summary>
        protected class GLFloat : GLSLObject {
            internal override string TypeName => "float";
            public GLFloat () { }
            public GLFloat (string name) : base (name) { }
        }

        /// <summary>
        /// Vec2.
        /// </summary>
        protected class Vec2 : GLSLObject {
            internal override string TypeName => "vec2";
            public Vec2 () { }
            public Vec2 (string name) : base (name) { }
        }

        /// <summary>
        /// Vec3.
        /// </summary>
        protected class Vec3 : GLSLObject {
            internal override string TypeName => "vec3";
            public Vec3 () { }
            public Vec3 (string name) : base (name) { }
        }

        /// <summary>
        /// Vec4.
        /// </summary>
        protected class Vec4 : GLSLObject {
            internal override string TypeName => "vec4";
            public Vec4 () { }
            public Vec4 (string name) : base (name) { }
        }

        /// <summary>
        /// Mat4.
        /// </summary>
        protected class Mat4 : GLSLObject {
            internal override string TypeName => "mat4";
            public Mat4 () { }
            public Mat4 (string name) : base (name) { }
        }

        /// <summary>
        /// Sampler2D.
        /// </summary>
        protected class Sampler2D : GLSLObject {
            internal override string TypeName => "sampler2D";
            public Sampler2D () { }
            public Sampler2D (string name) : base (name) { }
        }
    }
}

