using Linguini.Bundle.Builder;
using System.Globalization;
using megastar.Game.View;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Screens;

namespace megastar.Game
{
    public partial class MegastarGame : MegastarGameBase
    {
        private ScreenStack screenStack;

        [BackgroundDependencyLoader]
        private void load()
        {
            // Add your top-level game components here.
            // A screen stack and sample screen has been provided for convenience, but you can replace it if you don't want to use screens.
            Child = screenStack = new ScreenStack { RelativeSizeAxes = Axes.Both };

            var translationStore = new TranslationStore();

            LinguiniBuilder.Builder().CultureInfo(new CultureInfo("en-US")).AddResource(translationStore.Get("en-US"));
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            screenStack.Push(new MainScreen());
        }
    }
}
