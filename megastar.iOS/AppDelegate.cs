using osu.Framework.iOS;
using megastar.Game;

namespace megastar.iOS
{
    /// <inheritdoc />
    public class AppDelegate : GameApplicationDelegate
    {
        /// <inheritdoc />
        protected override osu.Framework.Game CreateGame() => new megastarGame();
    }
}
