using System;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq;
using System.Collections.Generic;

namespace libzeta {
    
    public abstract partial class ShaderBuilder {

        readonly StringBuilder builder;
        readonly internal BindingFlags generalFlags = 0x0
            | BindingFlags.Instance
            | BindingFlags.DeclaredOnly
            | BindingFlags.NonPublic;

        [Out] protected Vec4 gl_Position = new Vec4 { Name = "gl_Position" };

        protected ShaderBuilder () {
            builder = new StringBuilder ();
            structCache = new Dictionary<Type, Struct> ();
            GLSLObject.builder = builder;
            GLSLObject.structCache = structCache;
            reindent = true;
        }

        protected void Set (GLSLObject _out, GLSLObject _in) {
            BuildString ($"{_out.Name} = {_in.Name};\n");
        }

        /// <summary>
        /// Defines a constant with the specified name.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="code">Code.</param>
        protected GLSLObject Define (string name, string code) {
            BuildString ($"#define {name} {code}\n");
            return new GLSLObject (name);
        }

        /// <summary>
        /// The declaration type.
        /// </summary>
        protected enum Declaration {
            In,
            Uniform,
            Out,
            Smooth_Out,
        }

        protected static T Infer<T> () where T : GLSLObject, new() {
            return new T ();
        }
    }
}

