using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using megastar.Game.Track;
using megastar.Game.Translations;
using megastar.Game.WebConnectionQueue;
using megastar.Resources;
using osu.Framework.Allocation;
using osu.Framework.Configuration;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.IO.Stores;
using osu.Framework.Lists;
using osu.Framework.Localisation;
using osuTK;
using MegastarTrackMetadata = megastar.Game.Track.Megastar.MegastarTrackMetadata;

namespace megastar.Game;

[Cached]
public partial class MegastarGameBase : osu.Framework.Game
{
    // Anything in this class is shared between the test browser and the game implementation.
    // It allows for caching global dependencies that should be accessible to tests, or changing
    // the screen scaling for all components including the test browser and framework overlays.

    protected override Container<Drawable> Content { get; }
    private readonly List<Language> locales = [];

    //QUEWE
    public List<MegastarTrackMetadata> QueuedSongs { get; private set; } = new();
    public List<MegastarTrackMetadata> IndexedSongs { get; private set; } = new();

    public readonly LocalQueueServer LocalQueueServer = new();

    private readonly TrackLoader trackLoader;
    private readonly TrackRepository trackRepository;

    [Resolved] private FrameworkConfigManager config { get; set; } = null!;

    private NamespacedResourceStore<byte[]> translations = null!; // TODO

    protected MegastarGameBase()
    {
        // Ensure game and tests scale with window size and screen DPI.
        base.Content.Add(Content = new DrawSizePreservingFillContainer
        {
            // You may want to change TargetDrawSize to your "default" resolution, which will decide how things scale and position when using absolute coordinates.
            TargetDrawSize = new Vector2(1920, 1080)
        });

        trackRepository = new TrackRepository();
        trackLoader = new TrackLoader(trackRepository);
    }

    protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
    {
        Settings.Initialize(Host.Storage);

        var dependencies = new DependencyContainer(base.CreateChildDependencies(parent));

        dependencies.CacheAs(locales);
        dependencies.Cache(trackLoader);
        dependencies.Cache(trackRepository);

        return dependencies;
    }


    public void QueueSong(MegastarTrackMetadata metadata)
    {
        if (Settings.GetSettings().DuplicateItems.Value || !QueuedSongs.Contains(metadata))
        {
            QueuedSongs.Add(metadata);
        }


        _ = LocalQueueServer.BroadcastStateAsync();
    }

    /// <summary>
    /// This returns the next song of the queue and pop the current song
    /// If there is no song in the queue, it will return null
    /// If there is only one song in the queue, it returns it
    /// </summary>
    /// <returns>The next song in the queue</returns>
    public MegastarTrackMetadata? NextSong()
    {
        switch (QueuedSongs.Count)
        {
            case 0:
                return null;
            case 1:
                return QueuedSongs[0];
            default:
                QueuedSongs.RemoveAt(0);
                _ = LocalQueueServer.BroadcastStateAsync();
                return QueuedSongs[0];
        }
    }

    /// <summary>
    /// Returns the next song in the Queue (at the second position) if it exists. Returns the current one, if there is only one and null if there is none
    /// </summary>
    /// <returns>the song that is next in the queue</returns>
    public MegastarTrackMetadata? PeekNextSong()
    {
        return QueuedSongs.Count switch
        {
            0 => null,
            1 => QueuedSongs[0],
            _ => QueuedSongs[1]
        };
    }

    /// <summary>
    /// Should only be used to get the first song of the queue on startup.
    /// Returns the first song of the queue.
    /// For continuing with the next song, probably NextSong() should be used.
    /// Returns null if there is no song in the queue
    /// </summary>
    /// <returns></returns>
    public MegastarTrackMetadata? GetFirstSong() => QueuedSongs.Count > 0 ? QueuedSongs.First() : null;


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

        Resources.AddStore(new DllResourceStore(GetType().Assembly));

        AddFont(Resources, @"Fonts/standardFont");
        AddFont(Resources, @"Fonts/kuuhleFont");

        var allTracks = trackRepository.AllTracks().ToList();
        //TODO this sorting coud take some time, not noticable at the moment
        allTracks.Sort(TrackComparer.Instance);
        //TODO this removes all duets which are currently not parsable
        allTracks.RemoveAll(metadata => metadata.DirPath.ToLower().Contains("duet"));
        IndexedSongs = new List<MegastarTrackMetadata>(allTracks);
        //TODO Test purpose only
        //QueuedSongs = new List<MegastarTrackMetadata>(allTracks);
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
        if (string.IsNullOrEmpty(config.Get<string>(FrameworkSetting.Locale))) return;

        string systemLocale = CultureInfo.CurrentUICulture.Name;
        Language? systemLanguage = locales.Find(l => l.Code == systemLocale);

        config.SetValue(FrameworkSetting.Locale, systemLanguage == null ? "en-US" : systemLocale);
    }

    /// <summary>
    /// Adds a song into the indexed songs, without adding duplicates. Also keeps order of the list
    /// </summary>
    /// <param name="metadata"></param>
    public void AddIndexedSong(MegastarTrackMetadata metadata)
    {
        int index = IndexedSongs.BinarySearch(metadata, TrackComparer.Instance);
        if (index < 0)
        {
            IndexedSongs.Insert(~index, metadata);
        }
    }


    /// <summary>
    /// This comparer is used for ordering the tracks in the index list
    /// </summary>
    public class TrackComparer : IComparer<MegastarTrackMetadata>
    {
        public static readonly TrackComparer Instance = new();

        public int Compare(MegastarTrackMetadata? x, MegastarTrackMetadata? y)
        {
            if (ReferenceEquals(x, y)) return 0;
            if (x is null) return -1;
            if (y is null) return 1;

            // 1. Sort by Artist
            int artistComparison = string.Compare(x.Artist, y.Artist, StringComparison.OrdinalIgnoreCase);
            if (artistComparison != 0) return artistComparison;

            // 2. Sort by Title
            int titleComparison = string.Compare(x.Title, y.Title, StringComparison.OrdinalIgnoreCase);
            if (titleComparison != 0) return titleComparison;

            // 3. Tie-breaker by AudioPath
            return string.Compare(x.AudioFile, y.AudioFile, StringComparison.OrdinalIgnoreCase);
        }
    }
}
