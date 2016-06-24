using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace libzeta {

    /// <summary>
    /// Game.
    /// </summary>
    public abstract class Game : IDisposable {

        /// <summary>
        /// The game configuration.
        /// </summary>
        internal readonly GameConfiguration gameConfiguration;

        /// <summary>
        /// The components.
        /// </summary>
        internal HashSet<GameComponent> components;

        /// <summary>
        /// The user interface thread queue.
        /// </summary>
        ConcurrentQueue<Action> uiThreadQueue;

        /// <summary>
        /// The OpenGL graphics mode.
        /// </summary>
        GraphicsMode glGraphicsMode;

        /// <summary>
        /// The OpenGL context.
        /// </summary>
        GraphicsContext glContext;

        /// <summary>
        /// The cancellation token source.
        /// </summary>
        CancellationTokenSource tokenSource;

        /// <summary>
        /// The cancellation token.
        /// </summary>
        CancellationToken token;

        /// <summary>
        /// The game synchronizer.
        /// </summary>
        GameSynchronizer synchronizer;

        /// <summary>
        /// The game loop thread.
        /// </summary>
        Thread loopThread;

        /// <summary>
        /// The mouse.
        /// </summary>
        internal protected Mouse Mouse;

        /// <summary>
        /// The keyboard.
        /// </summary>
        internal protected Keyboard Keyboard;

        /// <summary>
        /// The native window.
        /// </summary>
        internal protected NativeWindow Window;

        /// <summary>
        /// The content manager.
        /// </summary>
        internal protected ContentManager Content;

        /// <summary>
        /// The sprite batch.
        /// </summary>
        internal protected SpriteBatch SpriteBatch;

        /// <summary>
        /// The rendering pipeline.
        /// </summary>
        internal protected RenderingPipeline Pipeline;

        /// <summary>
        /// The viewport.
        /// </summary>
        internal protected Viewport Viewport;

        /// <summary>
        /// Gets the resolution.
        /// </summary>
        /// <value>The resolution.</value>
        internal protected Resolution Resolution => new Resolution (Window?.Width ?? gameConfiguration.WindowWidth, Window?.Height ?? gameConfiguration.WindowHeight);

        /// <summary>
        /// Initializes a new instance of the <see cref="T:libzeta.Game"/> class.
        /// </summary>
        /// <param name="config">The game configuration.</param>
        protected Game (GameConfiguration? config = null) {
            gameConfiguration = config ?? GameConfiguration.Default;
        }

        /// <summary>
        /// Run the game.
        /// </summary>
        public void Run () {
            bool initialized = false;

            // Initialize the window
            this.Log (LoggingLevel.INFO, "Preparing the native window...", InitializeWindow);

            // Present the window to the user
            Window.Visible = true;

            // Wait until the window is visible
            while (!Window.Visible) {
                Thread.Sleep (0);
            }

            // Create the game loop thread
            loopThread = new Thread (() => {
                
                // Initialize OpenGL
                this.Log (LoggingLevel.INFO, "Initializing OpenGL...", InitializeOpenGL);

                // Initialize the game
                this.Log (LoggingLevel.INFO, "Preparing the game...", InitializeGame);

                // Call virtual initialization methods
                this.Log (LoggingLevel.INFO, "Running user initialization code...", () => {
                    LoadContent ();
                    Initialize ();
                    RegisterComponents ();
                });
                initialized = true;

                // Enter the game loop
                this.Log (LoggingLevel.INFO, "Entering the main game loop.");
                RunLoop ();

                // Do cleanup work
                Dispose ();
            });

            // Start the game loop thread
            loopThread.Start ();

            // Idle until the game is initialized
            while (!initialized) {
                Thread.Sleep (0);
            }

            // Process the message queue
            this.Log (LoggingLevel.INFO, "Entering the message processing loop.");
            this.Log (LoggingLevel.INFO, "The game is now running.");
            while (!token.IsCancellationRequested) {

                // Invoke waiting actions
                for (var i = 0; i < uiThreadQueue.Count; i++) {
                    Action action;
                    if (uiThreadQueue.TryDequeue (out action))
                        action ();
                }

                // Center the mouse
                Mouse.CenterMouse ();
                
                // Process window events
                Window.ProcessEvents ();
            }

            // Idle until the game loop is terminated
            while (loopThread.IsAlive) {
                Thread.Sleep (0);
            }
        }

        public void Exit () {
            Window.Close ();
        }

        /// <summary>
        /// Ensures than an action is ran inside of the ui thread.
        /// </summary>
        /// <param name="action">Action.</param>
        internal void EnsureUIThread (Action action) {
            uiThreadQueue.Enqueue (action);
        }

        /// <summary>
        /// Adds the specified game component.
        /// </summary>
        /// <param name="component">Component.</param>
        protected void AddComponent (GameComponent component) {
            if (!components.Contains (component)) {
                components.Add (component);
            }
        }

        /// <summary>
        /// Removes the specified game component.
        /// </summary>
        /// <param name="component">Component.</param>
        protected void RemoveComponent (GameComponent component) {
            if (components.Contains (component)) {
                components.Remove (component);
            }
        }

        /// <summary>
        /// Initialize class members here.
        /// </summary>
        internal protected virtual void Initialize () { }

        /// <summary>
        /// Register necessary game components here.
        /// </summary>
        internal protected virtual void RegisterComponents () { }

        /// <summary>
        /// Load assets here.
        /// </summary>
        internal protected virtual void LoadContent () { }

        /// <summary>
        /// Update the game logic here.
        /// Executes as fast as possible and possibly
        /// more than once during a single frame.
        /// </summary>
        /// <param name="time">Time.</param>
        internal protected virtual void Update (GameTime time) { }

        /// <summary>
        /// Update the game logic here.
        /// Guaranteed to run once per frame.
        /// </summary>
        /// <returns>The update.</returns>
        /// <param name="time">Time.</param>
        internal protected virtual void FixedUpdate (GameTime time) { }

        /// <summary>
        /// Draw the game graphics here.
        /// </summary>
        internal protected virtual void Draw () { }

        /// <summary>
        /// Initializes the window.
        /// </summary>
        /// <returns>The window.</returns>
        void InitializeWindow () {
            
            // Initialize the graphics mode to default
            glGraphicsMode = GraphicsMode.Default;

            // Start with default window flags
            var windowFlags = GameWindowFlags.Default;

            // Set the fixed window flag if requested
            if (gameConfiguration.FixedWindow) {
                windowFlags |= GameWindowFlags.FixedWindow;
            }

            // Set the fullscreen flag if requested
            if (gameConfiguration.Fullscreen) {
                windowFlags |= GameWindowFlags.Fullscreen;
            }

            // Create the native window
            Window = new NativeWindow (
                width: gameConfiguration.WindowWidth,
                height: gameConfiguration.WindowHeight,
                title: gameConfiguration.WindowTitle,
                options: windowFlags,
                mode: glGraphicsMode,
                device: DisplayDevice.Default
            );

            // Subscribe to window events
            Window.Resize += Window_Resize;
            Window.Closing += Window_Closing;
            Window.KeyDown += Window_KeyDown;
            Window.KeyUp += Window_KeyUp;

            // Create the ui thread queue
            uiThreadQueue = new ConcurrentQueue<Action> ();
        }

        /// <summary>
        /// Initializes OpenGL.
        /// The window is guaranteed to be available at this point.
        /// </summary>
        /// <returns>The open gl.</returns>
        void InitializeOpenGL () {

            // Create the OpenGL core context
            glContext = new GraphicsContext (
                mode: glGraphicsMode,
                window: Window.WindowInfo,
                major: 4,
                minor: 0,
                flags: GraphicsContextFlags.ForwardCompatible
            );

            // Make the newly created context current
            glContext.MakeCurrent (Window.WindowInfo);

            // Assert that the context is valid
            GraphicsContext.Assert ();

            // Load OpenGL entry points
            glContext.LoadAll ();

            // Set vertical synchronization mode
            switch (gameConfiguration.Vsync) {
            case VsyncMode.Off:
                glContext.SwapInterval = 0;
                break;
            case VsyncMode.On:
                glContext.SwapInterval = 1;
                break;
            case VsyncMode.Adaptive:
                glContext.SwapInterval = -1;
                break;
            }

            // Set defaults
            GL.Enable (EnableCap.Blend);
            GL.BlendFunc (BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.Enable (EnableCap.DepthTest);
            GL.ClearDepth (1.0f);
            GL.DepthFunc (DepthFunction.Lequal);
        }

        /// <summary>
        /// Initializes the game.
        /// The window and the OpenGL context are
        /// guaranteed to be available at this point.
        /// </summary>
        /// <returns>The game.</returns>
        void InitializeGame () {

            // Initialize thread safety stuff
            tokenSource = new CancellationTokenSource ();
            token = tokenSource.Token;

            // Create the component hashset
            components = new HashSet<GameComponent> ();

            // Create the game synchronizer
            synchronizer = new GameSynchronizer (this);

            // Create the content manager
            Content = new ContentManager ();

            // Create the sprite batch
            SpriteBatch = new SpriteBatch (this);

            // Create the viewport
            Viewport = new Viewport (Resolution);

            // Initialize input devices
            Mouse = new Mouse (Window, this, false, true);
            Keyboard = new Keyboard ();

            // Register internal components
            AddComponent (Mouse);

            // Create the rendering pipeline
            Pipeline = new RenderingPipeline (this);
        }

        /// <summary>
        /// Runs the game loop.
        /// </summary>
        void RunLoop () {

            // Initialize the fps counter
            var sw = Stopwatch.StartNew ();
            var fps = 0;

            // Enter the actual game loop
            while (!token.IsCancellationRequested) {

                // Update and synchronize the game
                synchronizer.Update ();

                // Update the fixed update logic
                synchronizer.FixedUpdate ();

                // Draw the game
                synchronizer.Draw ();

                // Swap buffers
                glContext.SwapBuffers ();

                // Compute the current fps value
                if (sw.Elapsed.TotalSeconds >= 1.0d) {
                    Console.Title = $"Zeta Engine - {fps}fps";
                    fps = 0;
                    sw.Restart ();
                } else {
                    ++fps;
                }
            }
        }

        /// <summary>
        /// Leave the game loop in a safe and clean way.
        /// </summary>
        /// <returns>The gracefully.</returns>
        void ExitGracefully () {
            tokenSource.Cancel ();
        }

        #region Event handlers

        /// <summary>
        /// Handles the Resize event of the native window.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">Event args.</param>
        void Window_Resize (object sender, EventArgs e) {

            // Update the OpenGL context
            glContext.Update (Window.WindowInfo);
        }

        /// <summary>
        /// Handles the Closing event of the native window.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">Event args.</param>
        void Window_Closing (object sender, CancelEventArgs e) {
            e.Cancel = true;
            this.Log (LoggingLevel.INFO, "Stopping the game...", ExitGracefully);
        }

        /// <summary>
        /// Handles the KeyDown event of the native window.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">Event args.</param>
        void Window_KeyDown (object sender, OpenTK.Input.KeyboardKeyEventArgs e) {
            Keyboard?.RegisterKeyDown (sender, e);
        }

        /// <summary>
        /// Handles the KeyUp event of the native window.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">Event args.</param>
        void Window_KeyUp (object sender, OpenTK.Input.KeyboardKeyEventArgs e) {
            Keyboard?.RegisterKeyUp (sender, e);
        }

        #endregion

        #region IDisposable Support
        bool disposedValue;

        protected virtual void Dispose (bool disposing) {
            if (!disposedValue) {
                if (disposing) {
                    SpriteBatch.Dispose ();
                    Pipeline.Dispose ();
                    Viewport.Dispose ();
                    tokenSource.Dispose ();
                    glContext.Dispose ();
                    Window.Dispose ();
                }
                uiThreadQueue = null;
                disposedValue = true;
            }
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="T:libzeta.Game"/> is reclaimed by garbage collection.
        /// </summary>
        ~Game() {
           Dispose(false);
        }

        // Correctly implement the disposable pattern.
        public void Dispose () {
            Dispose (true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}

