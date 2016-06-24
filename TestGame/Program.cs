using System;
using libzeta;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;

namespace TestGame {
    class MainClass {
        public static void Main (string [] args) {
            var config = GameConfiguration.Default;
            using (var game = new Zeta3DTestGame (config)) {
                game.Run ();
            }
        }
    }

    class Zeta3DTestGame : Game {

        FirstPersonCamera camera;
        Model suzanne;
        //MultiModel house;
        MultiModel zeta;

        public Zeta3DTestGame (GameConfiguration config) : base (config) {
        }

        protected override void Initialize () {

            // Lock the mouse
            Mouse.ShouldCenterMouse = true;
            Mouse.CursorVisible = false;
            
            // Enable face culling
            GL.CullFace (CullFaceMode.Back);
            GL.Enable (EnableCap.CullFace);

            // Create the camera
            camera = new FirstPersonCamera (this, Resolution.AspectRatio);
            camera.SetPosition (new Vector3 (0f, 0f, -40f));

            // Add a directional light
            var light = new DirectionalLight (Color4.White, new Vector3 (5, 5, 5), 0.5f);
            Pipeline.AddDirectionalLight (light);
        }

        protected override void RegisterComponents () {

            // Register the camera component
            AddComponent (camera);
        }

        protected override void LoadContent () {
            
            // Load suzanne
            suzanne = Content.Load<Model> ("suzanne.obj");
            zeta = Content.Load<MultiModel> ("zeta.fbx");
            zeta.SetPositions (
                new Vector3 (-10, 0, 0), new Vector3 (-10, 0, 0), new Vector3 (-10, 0, 0),
                new Vector3 (0, 0, 0), new Vector3 (0, 0, 0), new Vector3 (0, 0, 0),
                new Vector3 (10, 0, 0), new Vector3 (10, 0, 0), new Vector3 (10, 0, 0),
                new Vector3 (16, 0, 0), new Vector3 (16, 0, 0), new Vector3 (16, 0, 0)
            );

            // Load house
            //house = Content.Load<MultiModel> ("house.obj");
        }

        protected override void Update (GameTime time) {
            if (Keyboard.IsKeyTyped (Key.Escape)) {
                Exit ();
            }
        }

        protected override void Draw () {
            GL.Clear (ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.ClearColor (Color4.White);
            //GL.ClearColor (new Color4 (0.145098039215686f, 0.133333333333333f, 0.141176470588235f, 1f));
            Pipeline.Begin ();
            Pipeline.Draw (camera, zeta);
            Pipeline.End ();
            //Pipeline.Draw (camera, house);
        }
    }

    class ZetaTestGame : Game {

        OrthographicCamera camera;
        ScreenShake shake;
        Texture2D zeta;
        bool mouseDown;

        public ZetaTestGame (GameConfiguration config) : base (config) {
        }

        protected override void Initialize () {

            // Create the cameras
            camera = new OrthographicCamera (Window.Width, Window.Height);

            // Create the screen shake component
            shake = new ScreenShake (Resolution, camera);
        }

        protected override void RegisterComponents () {

            // Register the shake component
            AddComponent (shake);
        }

        protected override void LoadContent () {

            // Load the zeta logo
            zeta = Content.Load<Texture2D> ("zeta_transparent.png");
        }

        protected override void Update (GameTime time) {
            if (!mouseDown && Mouse.IsButtonDown (OpenTK.Input.MouseButton.Left)) {
                mouseDown = true;
                shake.Shake (2f, 25f, 1f);
            } else mouseDown &= !Mouse.IsButtonUp (OpenTK.Input.MouseButton.Left);
            if (Mouse.IsButtonDown (OpenTK.Input.MouseButton.Right)) {
                shake.ShakeStop ();
            }
            if (Keyboard.IsKeyTyped (OpenTK.Input.Key.Enter)) {
                shake.Shake (5f, 256f, 2f);
            }
        }

        protected override void Draw () {
            GL.Clear (ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.ClearColor (new Color4 (0.145098039215686f, 0.133333333333333f, 0.141176470588235f, 1f));
            SpriteBatch.Begin (camera);
            //SpriteBatch.Draw (background, Point2.Zero);
            SpriteBatch.Draw (zeta, Point2.Zero);
            SpriteBatch.End ();
        }
    }
}
