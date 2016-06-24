using System;
namespace libzeta {

    /// <summary>
    /// Game configuration.
    /// </summary>
    public struct GameConfiguration {
        
        /// <summary>
        /// Default game configuration.
        /// </summary>
        readonly public static GameConfiguration Default = new GameConfiguration {
            WindowWidth = 640,
            WindowHeight = 480,
            TargetFramerate = 60,
            Fullscreen = false,
            FixedWindow = true,
            FixedFramerate = true,
            WindowTitle = "Zeta Engine",
            Vsync = VsyncMode.Adaptive,
        };

        /// <summary>
        /// Width of the window.
        /// </summary>
        public int WindowWidth;

        /// <summary>
        /// Height of the window.
        /// </summary>
        public int WindowHeight;

        /// <summary>
        /// Target framerate.
        /// Only if FixedFramerate is set to true.
        /// </summary>
        public int TargetFramerate;

        /// <summary>
        /// Whether the game should start in fullscreen mode.
        /// </summary>
        public bool Fullscreen;

        /// <summary>
        /// Whether the window should be fixed or resizable.
        /// </summary>
        public bool FixedWindow;

        /// <summary>
        /// Whether the framerate should be fixed or variable.
        /// </summary>
        public bool FixedFramerate;

        /// <summary>
        /// The vertical synchronization mode.
        /// </summary>
        public VsyncMode Vsync;

        /// <summary>
        /// The window title.
        /// </summary>
        public string WindowTitle;
    }
}

