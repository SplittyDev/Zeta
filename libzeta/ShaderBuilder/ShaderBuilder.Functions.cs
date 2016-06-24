using System;
namespace libzeta {
    public abstract partial class ShaderBuilder {

        protected Vec3 vec3 (params string [] identifiers) {
            return new Vec3 ($"vec3 ({string.Join (" ", identifiers)})");
        }

        protected Vec4 vec4 (GLSLObject obj) {
            return new Vec4 ($"{obj}");
        }

        protected Vec4 vec4 (Vec3 vec, float scale) {
            return new Vec4 ($"vec4 ({vec}, {scale})");
        }

        protected Vec4 texture (Sampler2D sampler, Vec2 coords) {
            return new Vec4 ($"texture ({sampler}, {coords})");
        }

        protected Vec4 texture2d (Sampler2D sampler, Vec2 coords) {
            return new Vec4 ($"texture2d ({sampler}, {coords})");
        }
    }
}

