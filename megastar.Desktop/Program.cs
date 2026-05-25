using System;
using ManagedBass;
using megastar.Game;
using megastar.Game.Audio;
using osu.Framework;
using osu.Framework.Platform;

namespace megastar.Desktop
{
    public static class Program
    {
        public static void Main()
        {
            using GameHost host = Host.GetSuitableDesktopHost(@"megastar");
            using osu.Framework.Game game = new MegastarGame();

            MicManager micManager = new MicManager();
            micManager.AddMic(-1);
            micManager.StartAll();

            host.Run(game);
        }
    }
}
