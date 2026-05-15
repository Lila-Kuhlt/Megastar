using megastar.Game;
using osu.Framework;
using osu.Framework.Platform;

namespace megastar.Desktop
{
    public static class Program
    {
        public static void Main()
        {
            using (GameHost host = Host.GetSuitableDesktopHost(@"megastar"))
            using (osu.Framework.Game game = new MegastarGame())
                host.Run(game);
        }
    }
}
