using System;
using System.IO;
using megastar.Game.notes;
using megastar.Game.pitch;
using megastar.Game.Preset;
using megastar.Game.Track;
using megastar.Game.Track.Megastar;
using megastar.Game.Translations;
using OpenTabletDriver.Native.Windows.Input;
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
using osu.Framework.Input.Events;
using osu.Framework.IO.Stores;
using osu.Framework.Logging;
using osu.Framework.Platform;
using osu.Framework.Screens;
using osuTK.Input;
using osu.Framework.Timing;
using PitchTracking;
using AudioTrack = osu.Framework.Audio.Track.Track;
using ITrack = megastar.Game.Track.ITrack;

namespace megastar.Game.View;

public partial class PlayScreen : Screen
{
    private AudioTrack audioTrack;

    [Resolved] private MegastarGameBase game { get; set; } = null!;
    [Resolved] private GameHost host { get; set; } = null!;

    private Lyrics lyrics = null!;
    private LyricsContainer lyricsContainer = null!;
    private NoteContainer notesContainer = null!;


    private static AudioManager audioManager = null!;
    private ITrack? currentTrack;
    private Video backgroundVideo = null!;

    private double beat { get; set; }
    private bool paused = false;
    private double trackPausePosition = 0;

    private static Settings settings = Settings.GetSettings();


    private MicrophonePitchTracker[] micTrackers;
    private Colour4[] playerColours = { Colour4.LimeGreen, Colour4.Purple, Colour4.Coral, Colour4.Pink };
    private Lyric? currentDisplayedLyric;

    private INote lastReceivedNote = new UsdxNote(-1, -1, -1000, "error", UsdxNoteType.Sung);


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


    private Sprite? currentBackground;
    private TextureStore? activeTextureStore;
    private StorageBackedResourceStore? activeTextureResourceStore;
    private StorageBackedResourceStore? activeAudioResourceStore;
    private StorageBackedResourceStore? activeVideoRessourceStore;

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
        micTrackers = new MicrophonePitchTracker[settings.MicrophoneCount.Value];
        for (int i = 0; i < micTrackers.Length; i++)
        {
            micTrackers[i] = new MicrophonePitchTracker(i);
            micTrackers[i].PitchDetected += OnPitchDetected;
        }
    }

    public override void OnEntering(ScreenTransitionEvent e)
    {
        base.OnEntering(e);

        //TODO hier sollte irgendwie auch die nächsten Lieder abgespielt werden
        if (game.GetFirstSong() is { } song)
        {
            var track = new MegastarTrack(song);
            loadTrack(track);
            for (int i = 0; i < micTrackers.Length; i++)
            {
                micTrackers[i].Start(settings.GetBassDeviceIndexByName(settings.GetMicrophoneDevice(i)));
            }
        }
        else
            AddInternal(new SpriteText
            {
                Text = Fluent.Translate("play-song-error"),
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre
            });
    }

    private void loadTrack(ITrack track)
    {
        lyrics = new Lyrics(track.TrackData);

        var audio = loadAudio(track.Metadata.DirPath, track.Metadata.AudioFile);
        if (audio == null)
            return;

        audio.Start();
        audioTrack = audio;
        currentTrack = track;
        audioTrack.Looping = false;

        loadBackgroundImage(track.Metadata);
        loadBackgroundVideo(track.Metadata);

        audio.Volume.Value = Settings.GetSettings().SoundVolume.Value / 100f;

        currentDisplayedLyric = lyrics.LyricForBeat(0);

        if (currentDisplayedLyric == null)
        {
            Logger.Log("Tried to play track without lyrics", LoggingTarget.Input, LogLevel.Error);
            return;
        }

        showLyric(currentDisplayedLyric);
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

    private void loadBackgroundImage(ITrackMetadata usdxTrack)
    {
        if (!usdxTrack.BackgroundImageFile.IsNotNull()) return;

        try
        {
            // Create clean virtual storage handles targeting the song's directory
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

    private void loadBackgroundVideo(ITrackMetadata usdxTrack)
    {
        if (usdxTrack.BackgroundVideoFile.IsNull()) return;

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

            backgroundVideo.Clock = new FramedOffsetClock(audioTrack) { Offset = usdxTrack.VideoGap };
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
        if (currentTrack == null || currentDisplayedLyric == null) return;

        //End screen on track end
        if (audioTrack != null && audioTrack.HasCompleted && currentTrack != null && this.IsCurrentScreen())
        {
            var backgroundImage = currentTrack.Metadata.BackgroundImageFile.IsNotNull()
                ? activeTextureStore.Get(currentTrack.Metadata.BackgroundImageFile)
                : null;
            //TODO Real score needs to be entered here
            this.Push(new EndScreen(backgroundImage, currentTrack, 67911, 676767));
        }

        var ultraStarBpm = currentTrack.Metadata.Bpm;
        beat = ultraStarBpm * 4 * (audioTrack.CurrentTime - currentTrack.Metadata.Gap) / 60000.0;

        notesContainer.UpdateBeat(beat);
        lyricsContainer.UpdateBeat(beat);

        var nextLyric = lyrics.LyricAfterBeat(currentDisplayedLyric.StartBeat);
        //End of song or Error
        if (nextLyric == null) return;

        //Switch Phrase shortly before next
        var switchBeat = Math.Max(currentDisplayedLyric.EndBeat, nextLyric.StartBeat - 12);

        //TODO only for test purpose
        //if (audioTrack != null && Math.Abs(audioTrack.CurrentTime - audioTrack.Length) > 10000)
        //{
        //    audioTrack.Seek(audioTrack.Length - 4000);
        //    audioTrack.Looping = false;
        //}

        if (beat >= switchBeat)
        {
            currentDisplayedLyric = nextLyric;
            showLyric(currentDisplayedLyric);
        }
    }

    public override void OnResuming(ScreenTransitionEvent e)
    {
        base.OnResuming(e);

        //checks if the resume is from a pause screen or endscreen
        if (paused)
        {
            paused = false;
            MegastarTrackMetadata song = game.GetFirstSong();
            var track = new MegastarTrack(song);
            if (track.Metadata.AudioFile != currentTrack.Metadata.AudioFile)
            {
                loadTrack(track);
            }
            else
            {
                audioTrack?.Start();
            }
        }
        else
        {
            this.loadTrack(new MegastarTrack(game.NextSong()));
        }
    }

    private AudioTrack? loadAudio(string directoryPath, string fileName)
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
    public void ReceiveSungNote(UsdxNote sungNote)
    {
        if (beat <= lastReceivedNoteBeat) return;

        // Merge if same pitch
        if (lastReceivedNote != null && lastReceivedNote.Pitch == sungNote.Pitch)
        {
            lastReceivedNote.Length += sungNote.Length;
            lastReceivedNote.Visual.Width += (sungNote.Length * UsdxNote.SCALE_FACTOR);
            lastReceivedNoteBeat = sungNote.StartBeat + sungNote.Length;
        }
        else
        {
            // Pitch changed or it is the first note
            notesContainer.AddSungNote(sungNote);
            lastReceivedNoteBeat = sungNote.StartBeat + sungNote.Length;
            lastReceivedNote = sungNote;
        }
    }

    private void OnPitchDetected(PitchRecord record, int playerIndex)
    {
        if (currentTrack == null) return;

        //Thread saftey
        Schedule(() =>
        {
            //Offset of 60, as 60 in MIDI equals to 0 in USDX format
            int notePitch = record.MidiNote - 60;
            int currentBeat = (int)beat;


            var sungNote = new UsdxNote(currentBeat, 1, notePitch, "", UsdxNoteType.Sung, playerColours[playerIndex]);

            ReceiveSungNote(sungNote);
        });
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);
        foreach (var micTracker in micTrackers)
        {
            micTracker.Dispose();
        }

        //CLEANUP PREVIOUS SONG TRACK & RESOURCES
        audioTrack?.Stop();
        audioTrack?.Dispose();

        activeAudioResourceStore?.Dispose();
        activeVideoRessourceStore?.Dispose();

        // CLEANUP PREVIOUS BACKGROUND IMAGES & TEXTURE CACHES
        activeTextureResourceStore?.Dispose();
        activeTextureStore?.Dispose();
        currentBackground?.Dispose();
        backgroundLayer?.Dispose();
    }

    protected override bool OnKeyDown(KeyDownEvent e)
    {
        if (e.Key == Key.Escape)
        {
            this.Push(new PauseScreen());
            trackPausePosition = audioTrack.CurrentTime;
            audioTrack?.Stop();
            paused = true;
        }

        return base.OnKeyDown(e);
    }
}
