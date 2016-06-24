using System;
namespace libzeta {

    /// <summary>
    /// Game synchronizer.
    /// </summary>
    public class GameSynchronizer {

        /// <summary>
        /// The game.
        /// </summary>
        readonly Game game;

        /// <summary>
        /// The start time.
        /// </summary>
        readonly DateTime startTime;

        /// <summary>
        /// The last time.
        /// </summary>
        DateTime lastTime;

        /// <summary>
        /// Current time in seconds.
        /// </summary>
        public double CTS;

        /// <summary>
        /// Updated time in seconds.
        /// </summary>
        public double UTS;

        /// <summary>
        /// Accumulated time in seconds.
        /// </summary>
        public double ACS;

        /// <summary>
        /// Time delta in seconds.
        /// </summary>
        public double DTS;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:libzeta.GameSynchronizer"/> class.
        /// </summary>
        public GameSynchronizer (Game game) {
            this.game = game;
            startTime = DateTime.UtcNow;
            lastTime = startTime;
        }

        /// <summary>
        /// Synchronizes the game and updates the game logic.
        /// </summary>
        public void Update () {
            GameTime time;
            TimeSpan tts, ets;
            var now = DateTime.UtcNow;
            if (game.gameConfiguration.FixedFramerate) {
                var delta = 1d / game.gameConfiguration.TargetFramerate;
                UTS = DateTime.UtcNow.Subtract (startTime).TotalSeconds;
                DTS = UTS - CTS;
                CTS = UTS;
                ACS += DTS;
                while (ACS >= delta) {
                    now = DateTime.UtcNow;
                    tts = now.Subtract (startTime);
                    ets = now.Subtract (lastTime);
                    lastTime = now;
                    ACS -= delta;
                    time = new GameTime (ets, tts);
                    game.Update (time);
                    foreach (var component in game.components) {
                        component.Update (time);
                    }
                }
            } else {
                tts = now.Subtract (startTime);
                ets = now.Subtract (lastTime);
                lastTime = now;
                time = new GameTime (ets, tts);
                game.Update (time);
                foreach (var component in game.components) {
                    component.Update (time);
                }
            }
        }

        /// <summary>
        /// Updates the game logic once per frame.
        /// </summary>
        /// <returns>The update.</returns>
        public void FixedUpdate () {
            var now = DateTime.UtcNow;
            var tts = now.Subtract (startTime);
            var ets = now.Subtract (lastTime);
            var time = new GameTime (ets, tts);
            game.FixedUpdate (time);
            foreach (var component in game.components) {
                component.FixedUpdate (time);
            }
        }

        /// <summary>
        /// Draws the game graphics.
        /// </summary>
        public void Draw () {
            game.Draw ();
            foreach (var component in game.components) {
                component.Draw ();
            }
        }
    }
}

