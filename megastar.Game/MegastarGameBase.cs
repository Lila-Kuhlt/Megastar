using System;
using System.Collections.Generic;
using System.Linq;
using megastar.Game.Track;
using megastar.Game.Translations;
using megastar.Resources;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Textures;
using osu.Framework.Allocation;
using osu.Framework.IO.Stores;
using osuTK;

namespace megastar.Game
{
    [Cached]
    public partial class MegastarGameBase : osu.Framework.Game
    {
        // Anything in this class is shared between the test browser and the game implementation.
        // It allows for caching global dependencies that should be accessible to tests, or changing
        // the screen scaling for all components including the test browser and framework overlays.

        protected override Container<Drawable> Content { get; }
        private MsTranslationStore translationStore;

        protected MegastarGameBase()
        {
            // Ensure game and tests scale with window size and screen DPI.
            base.Content.Add(Content = new DrawSizePreservingFillContainer
            {
                // You may want to change TargetDrawSize to your "default" resolution, which will decide how things scale and position when using absolute coordinates.
                TargetDrawSize = new Vector2(1366, 768)
            });
        }

        /// <summary>
        /// Injects the translationStore into the cached dependencies, so it can be accessed like this:
        /// <code>
        /// [BackgroundDependencyLoader]
        /// private void load(TranslationStore translations)
        /// </code>
        /// </summary>
        /// <param name="parent">
        /// The parent DependencyContainer
        /// </param>
        protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
        {
            Settings.Initialize(Host.Storage);

            var dependencies = new DependencyContainer(base.CreateChildDependencies(parent));
            string savedLanguage = Settings.GetSettings().Language.Value;
            translationStore = new MsTranslationStore(new DllResourceStore(typeof(MegastarResources).Assembly), savedLanguage);

            Settings.GetSettings().Language.ValueChanged += e =>
            {
                translationStore.SetLanguage(e.NewValue);
            };

            dependencies.CacheAs(translationStore);
            return dependencies;
        }

        public List<UsdxTrack> LoadedSongs { get; private set; } = [];

        //QUEWE
        public static Queue<UsdxTrack> QueuedSongs { get; private set; } = new Queue<UsdxTrack>();


        [BackgroundDependencyLoader]
        private void load()
        {
            // disable tablet discovery
            Host.Storage.GetStorageForDirectory("config");
            var localConfig = Host.AvailableInputHandlers;
            foreach (var handler in Host.AvailableInputHandlers.Where(handler =>
                         handler.GetType().Name.Contains("Tablet", StringComparison.OrdinalIgnoreCase)))
                handler.Enabled.Value = false;

            Resources.AddStore(new DllResourceStore(typeof(MegastarResources).Assembly));

        }
    }
}
