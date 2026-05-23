using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using megastar.Game.Track;
using megastar.Game.Translations;
using megastar.Game.View;
using megastar.Resources;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Textures;
using osu.Framework.Allocation;
using osu.Framework.Configuration;
using osu.Framework.IO.Stores;
using osu.Framework.Localisation;
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
        private readonly List<string> locales = new List<string>();

        [Resolved]
        private FrameworkConfigManager config { get; set; }

        private IResourceStore<byte[]> translations;

        protected MegastarGameBase()
        {
            // Ensure game and tests scale with window size and screen DPI.
            base.Content.Add(Content = new DrawSizePreservingFillContainer
            {
                // You may want to change TargetDrawSize to your "default" resolution, which will decide how things scale and position when using absolute coordinates.
                TargetDrawSize = new Vector2(1366, 768)
            });
        }

        protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
        {
            var dependencies = new DependencyContainer(base.CreateChildDependencies(parent));

            dependencies.CacheAs(locales);
            return dependencies;
        }

        public List<UsdxTrack> LoadedSongs { get; private set; } = [];

        //QUEWE
        public List<UsdxTrack> QueuedSongs { get; private set; } = new List<UsdxTrack>();

        [BackgroundDependencyLoader]
        private void load()
        {
            // disable tablet discovery
            Host.Storage.GetStorageForDirectory("config");
            var localConfig = Host.AvailableInputHandlers;
            foreach (var handler in Host.AvailableInputHandlers.Where(handler =>
                         handler.GetType().Name.Contains("Tablet", StringComparison.OrdinalIgnoreCase)))
                handler.Enabled.Value = false;

            IResourceStore<byte[]> resourceAssembly = new DllResourceStore(typeof(MegastarResources).Assembly);
            Resources.AddStore(resourceAssembly);

            translations = new NamespacedResourceStore<byte[]>(resourceAssembly, "Translations");
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            LocalisationManager localisation = Dependencies.Get<LocalisationManager>();

            foreach (var file in translations.GetAvailableResources())
            {
                string lang = Regex.Replace(file, "\\.ftl$", "");
                ILocalisationStore store = new FluentTranslationStore(translations, lang);
                localisation.AddLanguage(lang, store);
                locales.Add(lang);
            }

            if (config.Get<string>(FrameworkSetting.Locale) == null || config.Get<string>(FrameworkSetting.Locale) == "")
            {
                string systemLocale = CultureInfo.CurrentUICulture.Name;

                config.SetValue(FrameworkSetting.Locale, locales.Contains(systemLocale) ? systemLocale : "en-US");
            }
        }
    }
}
