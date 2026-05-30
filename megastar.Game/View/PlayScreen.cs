using System;
using System.IO;
using megastar.Game.notes;
using megastar.Game.Preset;
using megastar.Game.Track;
using megastar.Game.Translations;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Audio.Track;
using osu.Framework.Extensions.ObjectExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Graphics.Video;
using osu.Framework.IO.Stores;
using osu.Framework.Logging;
using osu.Framework.Platform;
using osu.Framework.Screens;
using osu.Framework.Timing;
using AudioTrack = osu.Framework.Audio.Track.Track;

namespace megastar.Game.View;

public partial class PlayScreen : Screen
{
    private AudioTrack track;

    [Resolved] private MegastarGameBase game { get; set; } = null!;
    [Resolved] private GameHost host { get; set; } = null!;

    private Lyrics lyrics = null!;
    private LyricsContainer lyricsContainer = null!;
    private NoteContainer notesContainer = null!;


    private static AudioManager audioManager = null!;
    private UsdxTrack currentTrack = null!;
    private Video backgroundVideo = null!;

    public double Beat { get; private set; }


    // Dedicated layer to safely swap background sprites behind UI elements
    private readonly Container backgroundLayer = new() { RelativeSizeAxes = Axes.Both };

    private readonly Container lyricsLayer = new()
    {
        RelativeSizeAxes = Axes.Both,
        Padding = new MarginPadding { Bottom = 50 }
    };

    private readonly Container notesLayer = new()
    {
        RelativeSizeAxes = Axes.Both,
        Anchor = Anchor.Centre,
        Origin = Anchor.Centre,
        AlwaysPresent = true
    };


    private Sprite currentBackground;
    private TextureStore activeTextureStore;
    private StorageBackedResourceStore activeTextureResourceStore;
    private StorageBackedResourceStore activeAudioResourceStore;
    private StorageBackedResourceStore activeVideoRessourceStore;
    private FluentTranslationStore t = null!;

    private int lastReceivedNoteBeat;

    [BackgroundDependencyLoader]
    private void load(AudioManager audio)
    {
        audioManager = audio;

        InternalChildren =
        [
            new Box { Colour = StandardColours.BACKGROUND, RelativeSizeAxes = Axes.Both },
            backgroundLayer,
            new BackButton(this.Exit, Fluent.Translate("common-back")),
            notesLayer,
            lyricsLayer
        ];
    }

    public override void OnEntering(ScreenTransitionEvent e)
    {
        base.OnEntering(e);

        //TODO hier sollte irgendwie auch die nächsten Lieder abgespielt werden
        try
        {
            if (game.NextSong() is { } song)
                loadTrack(song);
        }
        catch (Exception exception)
        {
            Logger.Error(exception, exception.Message);
            AddInternal(new SpriteText
            {
                Text = Fluent.Translate("play-song-error"),
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
            });
        }
    }

    private void loadTrack(UsdxTrack usdxTrack)
    {
        lyrics = new Lyrics(usdxTrack);

        var loadedTrack = loadSong(usdxTrack.DirPath, usdxTrack.SongFile);
        if (loadedTrack == null)
            return;

        track = loadedTrack;
        loadedTrack?.Start();

        loadBackgroundImage(usdxTrack);
        loadBackgroundVideo(usdxTrack);
        currentTrack = usdxTrack;
        if (loadedTrack != null) loadedTrack.Volume.Value = Settings.GetSettings().SoundVolume.Value / 100f;

        var currentLyric = lyrics.LyricForBeat((int)Beat);
        if (currentLyric == null)
        {
            Logger.Log("Tried to play track without lyrics", LoggingTarget.Input, LogLevel.Error);
            return;
        }

        showLyric(currentLyric);
    }


    private void showLyric(Lyric lyric)
    {
        lyricsLayer.Clear();
        notesLayer.Clear();

        lyricsContainer = new LyricsContainer(lyric)
        {
            Anchor = Anchor.BottomCentre,
            Origin = Anchor.BottomCentre
        };

        lyricsLayer.Add(lyricsContainer);

        notesContainer = new NoteContainer(lyric.Notes);
        notesLayer.Add(notesContainer);
    }

    private void loadBackgroundImage(UsdxTrack usdxTrack)
    {
        if (!usdxTrack.BackgroundImageFile.IsNotNull()) return;

        try
        {
            // Create clean virtual storage handles targetting the song's directory
            var textureStorage = new NativeStorage(usdxTrack.DirPath, host);
            activeTextureResourceStore = new StorageBackedResourceStore(textureStorage);
            activeTextureStore = new TextureStore(host.Renderer,
                host.CreateTextureLoaderStore(activeTextureResourceStore));

            var texture = activeTextureStore.Get(usdxTrack.BackgroundImageFile);

            if (texture == null) return;

            backgroundLayer.Add(currentBackground = new Sprite
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                FillMode = FillMode.Fill,
                Texture = texture,
                Alpha = 0
            });

            // Fade between backgrounds
            currentBackground.FadeIn(100, Easing.OutQuint);
        }
        catch (Exception ex) // TODO: Don't catch all Exceptions -- this is bad practice!
        {
            Logger.Error(ex, "Failed to load karaoke track background image.");
        }
    }

    private void loadBackgroundVideo(UsdxTrack usdxTrack)
    {
        if (!usdxTrack.BackgroundVideoFile.IsNotNull()) return;

        try
        {
            string videoPath = Path.Combine(usdxTrack.DirPath,
                usdxTrack.BackgroundVideoFile);

            if (!File.Exists(videoPath)) return;

            // Let C# handle the file reading safely to bypass FFmpeg pathing issues
            Stream videoStream = File.OpenRead(videoPath);

            backgroundVideo = new Video(videoStream)
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                FillMode = FillMode.Fill,
                Alpha = 0,
                Loop = false
            };

            backgroundVideo.Clock = new FramedOffsetClock(track) { Offset = usdxTrack.VideoGap };
            backgroundLayer.Add(backgroundVideo);
            backgroundVideo.OnLoadComplete += v => { v.FadeIn(0, Easing.OutQuint); };
        }
        catch (Exception ex) // TODO: Don't catch all Exceptions -- this is bad practice!
        {
            Logger.Error(ex, $"Failed to load karaoke track background video: {ex.Message}");
        }
    }

    protected override void Update()
    {
        base.Update();
        int iBeat = (int)Beat;

        ReceiveSungNote(new UsdxNote(iBeat, Random.Shared.Next(1, 5), Random.Shared.Next(5, 20), "",
            UsdxNoteType.Sung));

        double ultraStarBpm = currentTrack.Bpm;
        Beat = ultraStarBpm * 4 * (track.CurrentTime - currentTrack.Gap) / 60000.0;

        notesContainer.UpdateBeat(Beat);
        lyricsContainer.UpdateBeat(Beat);

        var currentLyric = lyrics.LyricForBeat(iBeat);
        var nextLyric = lyrics.LyricAfterBeat(iBeat);

        if (currentLyric == null || nextLyric == null) return;

        var endBeat = currentLyric.EndBeat;
        var startBeat = nextLyric.StartBeat;

        // Switch phrase 1/4 between the end of the current one and the start of the next one
        double switchBeat = endBeat + (endBeat - startBeat) / 4.0;

        if (!(Beat >= switchBeat)) return;

        showLyric(nextLyric);
    }

    private AudioTrack? loadSong(string directoryPath, string fileName)
    {
        try
        {
            var storage = new NativeStorage(directoryPath, host);
            activeAudioResourceStore = new StorageBackedResourceStore(storage);
            ITrackStore customTrackStore = audioManager.GetTrackStore(activeAudioResourceStore);
            return customTrackStore.Get(fileName);
        }
        catch (Exception ex) // TODO: Don't catch all Exceptions -- this is bad practice!
        {
            Logger.Error(ex, "Failed to load karaoke track audio.");
            return null;
        }
    }

    /// <summary>
    /// This method takes notes that get sung and displays them above the pitches
    /// This automatically only receives the first note per beat and ignores all following ones
    /// </summary>
    /// <param name="sungNote"></param>
    public void ReceiveSungNote(INote sungNote)
    {
        if (Beat <= lastReceivedNoteBeat) return;

        notesContainer.AddSungNote(sungNote);
        lastReceivedNoteBeat = sungNote.StartBeat + sungNote.Length;
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);

        //CLEANUP PREVIOUS SONG TRACK & RESOURCES
        track?.Stop();
        track?.Dispose();

        activeAudioResourceStore?.Dispose();
        activeVideoRessourceStore?.Dispose();

        // CLEANUP PREVIOUS BACKGROUND IMAGES & TEXTURE CACHES
        activeTextureResourceStore?.Dispose();
        activeTextureStore?.Dispose();
        currentBackground?.Dispose();
        backgroundLayer?.Dispose();
    }
}
