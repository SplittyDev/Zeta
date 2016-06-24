using System;
namespace libzeta {

    /// <summary>
    /// Shader collection.
    /// </summary>
    public static class ShaderCollection {

        /// <summary>
        /// SpriteBatch shaders.
        /// Used by the <see cref="SpriteBatch"/>.
        /// </summary>
        public static class SpriteBatch {

            public const string VertexShader = @"
            #version 400
            in vec3 v_pos;
            in vec4 v_col;
            in vec2 v_tex;
            out vec4 f_col;
            out vec2 f_tex;
            uniform mat4 MVP;
            
            void main () {
                f_col = v_col;
                f_tex = v_tex;
                gl_Position = MVP * vec4 (v_pos, 1.0);
            }";

            public const string FragmentShader = @"
            #version 400
            in vec4 f_col;
            in vec2 f_tex;
            out vec4 frag_color;
            uniform sampler2D tex;
            
            void main () {
                frag_color = texture (tex, f_tex) * f_col;
            }";
        }

        /// <summary>
        /// Viewport shaders.
        /// Used by the <see cref="Viewport"/>.
        /// </summary>
        public static class Viewport {

            public const string VertexShader = @"
            #version 400
            in vec3 v_pos;
            in vec2 v_tex;
            out vec2 f_tex;
            uniform mat4 MVP;

            void main () {
                f_tex = v_tex;

                gl_Position = MVP * vec4(v_pos, 1.0);
            }";

            public const string FragmentShader = @"
            #version 400
            in vec2 f_tex;
            uniform sampler2D tex;
            out vec4 frag_color;

            void main () {
                frag_color = texture (tex, f_tex);
            }";
        }

        /// <summary>
        /// Lighting shaders.
        /// Used by the <see cref="RenderingPipeline"/>.
        /// </summary>
        public static class Lighting {

            public const string AmbientVertexShader = @"
            #version 400
            in vec3 v_pos;
            in vec2 v_tex;
            uniform mat4 MVP;
            out vec2 f_tex;
            void main () {
                f_tex = v_tex;
                gl_Position = MVP * vec4 (v_pos, 1.0);
            }";

            public const string AmbientFragmentShader = @"
            #version 400
            struct Material {
                vec4 color;
                sampler2D diffuse;
                float specularIntensity;
                float specularPower;
            };

            in vec2 f_tex;
            uniform Material material;
            uniform vec3 ambient_color;
            out vec4 frag_color;

            void main () {
                frag_color = material.color * texture2D (material.diffuse, f_tex) * vec4 (ambient_color, 1.0);
            }";

            public const string DirectionalVertexShader = @"
            #version 400
            in vec3 v_pos;
            in vec2 v_tex;
            in vec3 v_nrm;
            uniform mat4 MVP;
            uniform mat4 NRM;
            out vec2 f_tex;
            out vec3 f_pos;
            smooth out vec3 f_nrm;

            void main () {
                vec2 tex = vec2(0);
                tex.x = v_tex.x;
                tex.y = 1 - v_tex.y;
                f_tex = tex;
                f_nrm = (NRM * vec4 (v_nrm, 0.0)).xyz;
                f_pos = (NRM * vec4 (v_pos, 1.0)).xyz;
                gl_Position = MVP * vec4 (v_pos, 1.0);
            }";

            public const string DirectionalFragmentShader = @"
            #version 400
            struct BaseLight {
                vec3 color;
                float intensity;
            };

            struct DirectionalLight {
                BaseLight base;
                vec3 direction;
            };

            struct Material {
                vec4 color;
                sampler2D diffuse;
                float specularIntensity;
                float specularPower;
            };

            in vec2 f_tex;
            in vec3 f_nrm;
            in vec3 f_pos;
            uniform Material material;
            uniform vec3 eye_pos;
            uniform DirectionalLight directionalLight;
            out vec4 frag_color;

            #define PI 3.141592653589793
            const float gamma = 2.2;

            // Beckmann distribution function
            float beckmannDistribution (float x, float roughness) {
              float NdotH = max (x, 0.0001);
              float cos2Alpha = NdotH * NdotH;
              float tan2Alpha = (cos2Alpha - 1.0) / cos2Alpha;
              float roughness2 = roughness * roughness;
              float denom = PI * roughness2 * cos2Alpha * cos2Alpha;
              return exp (tan2Alpha / roughness2) / denom;
            }
            
            // Cook-Torrance specular function
            float cookTorranceSpecular (
              vec3 lightDirection,
              vec3 viewDirection,
              vec3 surfaceNormal,
              float roughness,
              float fresnel) {

              // View and light points
              float VdotN = max (dot (viewDirection, surfaceNormal), 0.0);
              float LdotN = max (dot (lightDirection, surfaceNormal), 0.0);

              // Half angle vector
              vec3 H = normalize (lightDirection + viewDirection);

              // Geometric term
              float NdotH = max (dot (surfaceNormal, H), 0.0);
              float VdotH = max (dot (viewDirection, H), 0.000001);
              float LdotH = max (dot (lightDirection, H), 0.000001);
              float G1 = (2.0 * NdotH * VdotN) / VdotH;
              float G2 = (2.0 * NdotH * LdotN) / LdotH;
              float G = min (1.0, min (G1, G2));

              // Distribution term
              float D = beckmannDistribution (NdotH, roughness);

              // Fresnel term
              float F = pow (1.0 - VdotN, fresnel);

              // Multiply terms
              return  G * F * D / max (PI * VdotN, 0.000001);
            }

            // Calculate the light
            vec4 calcLight (BaseLight base, vec3 direction, vec3 normal) {
                float diffuseFactor = dot (normal, -direction);
                vec4 diffuseColor = vec4 (0);
                vec4 specularColor = vec4 (0);
                if (diffuseFactor > 0) {
                    diffuseColor = vec4 (base.color, 1.0) * base.intensity * diffuseFactor;
                    vec3 directionToEye = normalize (eye_pos - f_pos);
                    vec3 halfDir = normalize (directionToEye - direction);
                    float specularFactor = dot (halfDir, normal);
                    specularFactor = pow (specularFactor, material.specularPower);
                    specularFactor = cookTorranceSpecular (-direction, directionToEye, normal, .5, 1);
                    if (specularFactor > 0) {
                        specularColor = vec4 (base.color, 1.0) * material.specularIntensity * specularFactor;
                    }
                }
                return diffuseColor + specularColor;
            }

            // Calculate the directional light
            vec4 calcDirectionalLight (DirectionalLight light, vec3 normal) {
                return calcLight (light.base, -light.direction, normal);
            }

            float toLinear (float v) {
              return pow (v, gamma);
            }

            vec2 toLinear (vec2 v) {
              return pow (v, vec2 (gamma));
            }

            vec3 toLinear (vec3 v) {
              return pow (v, vec3 (gamma));
            }

            vec4 toLinear (vec4 v) {
              return vec4 (toLinear (v.rgb), v.a);
            }

            float toGamma (float v) {
              return pow (v, 1.0 / gamma);
            }

            vec2 toGamma (vec2 v) {
              return pow (v, vec2 (1.0 / gamma));
            }

            vec3 toGamma (vec3 v) {
              return pow (v, vec3 (1.0 / gamma));
            }

            vec4 toGamma (vec4 v) {
              return vec4 (toGamma (v.rgb), v.a);
            }

            void main () {
                vec4 color = toLinear (material.color * texture (material.diffuse, f_tex) * calcDirectionalLight (directionalLight, normalize (f_nrm)));
                frag_color = toGamma (color);
            }";
        }
    }
}

