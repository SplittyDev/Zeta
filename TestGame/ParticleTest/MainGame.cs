using System;
using libzeta;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace ParticleTest {

    /// <summary>
    /// Main game.
    /// </summary>
    public class MainGame : Game {
        
        ParticleEmitter emitter;
        DirectionModifier directionMod;
        OrthographicCamera camera;
        ScreenShake shake;
        Texture2D ship;
        Texture2D space;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ParticleTest.MainGame"/> class.
        /// </summary>
        /// <param name="conf">Configuration.</param>
        public MainGame (GameConfiguration conf) : base (conf) {
        }

        protected override void Initialize () {
            camera = new OrthographicCamera (Resolution);
            shake = new ScreenShake (Resolution, camera);
            emitter = new ParticleEmitter (this);
            emitter.Position = new Vector3 (0, 0, 0);
            emitter.AddModifier (new TextureModifier (
                true,
                Content.Load<Texture2D> ("fire0.png"),
                Content.Load<Texture2D> ("fire1.png"),
                Content.Load<Texture2D> ("fire2.png"),
                Content.Load<Texture2D> ("fire3.png")
            ));
            emitter.AddModifier (directionMod = new DirectionModifier (Vector3.UnitY));
            emitter.AddModifier (new SpreadModifier (true, false, 7));
            emitter.AddModifier (new AlphaModifier (1f, true));
            emitter.AddModifier (LifetimeModifier.CreateRandomMultiply ());
            emitter.SetSpeed (200);
            emitter.SetAmount (100);
            emitter.SetLifetime (0.5f);
            emitter.Start ();
        }

        protected override void LoadContent () {
            ship = Content.Load<Texture2D> ("ship.png");
            space = Content.Load<Texture2D> ("space.png");
        }

        protected override void RegisterComponents () {
            AddComponent (shake);
        }

        protected override void Update (GameTime time) {
            emitter.Update (time);
            if (Mouse.IsButtonDown (OpenTK.Input.MouseButton.Left)) {
                emitter.Position = new Vector3 (Mouse.X, Mouse.Y, 0);
                shake.Shake (2f, 2f, 1f);
            }
            if (Mouse.IsButtonDown (OpenTK.Input.MouseButton.Right)) {
                directionMod.Direction = new Vector3 (Mouse.X / Resolution.Width - 0.5f, Mouse.Y / Resolution.Height - 0.5f, 0);
            }
        }

        protected override void Draw () {
            GL.Clear (ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            SpriteBatch.TryBegin (camera);
            SpriteBatch.Draw (space, Point2.Zero);
            emitter.Draw ();
            SpriteBatch.Draw (ship, new Point2 (emitter.Position + new Vector3 (-(ship.Width / 2.5f), -(ship.Height * 0.5f), 0)));
            SpriteBatch.TryEnd ();
        }
    }
}

