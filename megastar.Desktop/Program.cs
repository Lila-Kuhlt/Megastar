using osu.Framework.Platform;
using osu.Framework;
using megastar.Game;

namespace megastar.Desktop
{
    public static class Program
    {
        public static void Main()
        {
            using (GameHost host = Host.GetSuitableDesktopHost(@"megastar"))
            using (osu.Framework.Game game = new megastarGame())
                host.Run(game);
        }
    }
}
