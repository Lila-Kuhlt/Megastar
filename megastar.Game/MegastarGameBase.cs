#nullable enable

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
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
using osu.Framework.Extensions.ObjectExtensions;
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
        private readonly List<Language> locales = new List<Language>();

        [Resolved] private FrameworkConfigManager config { get; set; }

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
            Settings.Initialize(Host.Storage);

            var dependencies = new DependencyContainer(base.CreateChildDependencies(parent));

            dependencies.CacheAs(locales);
            return dependencies;
        }


        public List<UsdxTrack> LoadedSongs { get; private set; } = new List<UsdxTrack>();

        //QUEWE
        public List<UsdxTrack> QueuedSongs { get; private set; } = new List<UsdxTrack>();

        public LocalQueueServer LocalQueueServer = new LocalQueueServer();


        public void QueueSong(UsdxTrack track)
        {
            if (Settings.GetSettings().DuplicateItems.Value || !QueuedSongs.Contains(track))
            {
                QueuedSongs.Add(track);
            }


            if (LocalQueueServer != null)
            {
                LocalQueueServer.BroadcastStateAsync();
            }
        }

        /// <summary>
        /// This returns the next song of the queue and pop the current song
        /// If there is no song in the queue, it will return null
        /// If there is only one song in the queue, it returns it
        /// </summary>
        /// <returns>The next song in the queue</returns>
        [CanBeNull]
        public UsdxTrack? NextSong()
        {
            switch (QueuedSongs.Count)
            {
                case 0:
                    return null;
                case 1:
                    return QueuedSongs[0];
                default:
                    QueuedSongs.RemoveAt(0);
                    this.LocalQueueServer.BroadcastStateAsync();
                    return QueuedSongs[0];
            }
        }

        /// <summary>
        /// Returns the next song in the Queue (at the second position) if it exists. Returns 0 if there are less then 2 tracks in the queue
        /// </summary>
        /// <returns>the song that is next in the queue</returns>
        public UsdxTrack? PeakNextSong()
        {
            switch (QueuedSongs.Count)
            {
                case 0:
                    return null;
                case 1:
                    return QueuedSongs[0];
                default:
                    return QueuedSongs[1];
            }
        }

        /// <summary>
        /// Should only be used to get the first song of the queue on startup.
        /// Returns the first song of the queue.
        /// For continuing with the next song, probably NextSong() should be used.
        /// Returns null if there is no song in the queue
        /// </summary>
        /// <returns></returns>
        public UsdxTrack? GetFirstSong()
        {
            switch (QueuedSongs.Count)
            {
                case 0:
                    return null;
                default:
                    return  QueuedSongs[0];
            }
        }


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
                string lang = Path.ChangeExtension(file, null);
                ILocalisationStore store = new FluentTranslationStore(translations, lang);
                localisation.AddLanguage(lang, store);
                Language language = new Language(lang, Fluent.Translate(lang), localisation);
                locales.Add(language);
            }

            // FrameworkSetting.Locale will be "" if the selected language is the system default language, since the framework does not persist the default language to file.
            // Why exactly it then does not load the system default language into the locale config on startup if it is empty is beyond me.
            if (config.Get<string>(FrameworkSetting.Locale) == null ||
                config.Get<string>(FrameworkSetting.Locale) == "")
            {
                string systemLocale = CultureInfo.CurrentUICulture.Name;
                Language systemLanguage = locales.Find(l => l.Code == systemLocale);

                if (systemLanguage == null)
                {
                    config.SetValue(FrameworkSetting.Locale, "en-US");
                }
                else
                {
                    config.SetValue(FrameworkSetting.Locale, systemLocale);
                }
            }
        }
    }
}
