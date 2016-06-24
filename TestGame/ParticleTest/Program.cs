using System;
using System.Threading;
using System.Threading.Tasks;
using libzeta;

namespace ParticleTest {
    class MainClass {
        public static void Main (string [] args) {
            using (var game = new MainGame (GameConfiguration.Default)) {
                game.Run ();
            }
        }
    }
}
