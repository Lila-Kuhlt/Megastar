using System;
using System.IO;
using megastar.Game.notes;
using megastar.Game.Preset;
using megastar.Game.Track;
using megastar.Game.Track.Megastar;
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
using ITrack = megastar.Game.Track.ITrack;

namespace megastar.Game.View;

public partial class PlayScreen : Screen
{
    [Resolved] private MegastarGameBase game { get; set; } = null!;
    [Resolved] private GameHost host { get; set; } = null!;
    [Resolved] private AudioManager audioManager { get; set; } = null!;

    private Lyrics lyrics = null!;
    private LyricsContainer lyricsContainer = null!;
    private NoteContainer notesContainer = null!;


    private ITrack? currentTrack;

    private AudioTrack audioTrack = null!;


    private double beat { get; set; }

    private int lastReceivedNoteBeat;

    private readonly DynamicTrackBackground background = new();

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

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChildren =
        [
            background,
            new BackButton(this.Exit, Fluent.Translate("common-back")),
            notesLayer,
            lyricsLayer
        ];
    }

    public override void OnEntering(ScreenTransitionEvent e)
    {
        base.OnEntering(e);

        //TODO hier sollte irgendwie auch die nächsten Lieder abgespielt werden
        if (game.NextSong() is { } song)
        {
            var track = new MegastarTrack(song);
            loadTrack(track);
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

        background.LoadTrack(track.Metadata, audio);

        audio.Start();

        audioTrack = audio;
        currentTrack = track;


        audio.Volume.Value = Settings.GetSettings().SoundVolume.Value / 100f;

        var currentLyric = lyrics.LyricForBeat((int)beat);
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

    protected override void Update()
    {
        base.Update();
        if (currentTrack == null) return;

        var iBeat = (int)beat;

        ReceiveSungNote(new UsdxNote(iBeat, Random.Shared.Next(1, 5), Random.Shared.Next(5, 20), "",
            UsdxNoteType.Sung));

        var ultraStarBpm = currentTrack.Metadata.Bpm;
        beat = ultraStarBpm * 4 * (audioTrack.CurrentTime - currentTrack.Metadata.Gap) / 60000.0;

        notesContainer.UpdateBeat(beat);
        lyricsContainer.UpdateBeat(beat);

        var currentLyric = lyrics.LyricForBeat(iBeat);
        var nextLyric = lyrics.LyricAfterBeat(iBeat);

        if (currentLyric == null || nextLyric == null) return;

        var endBeat = currentLyric.EndBeat;
        var startBeat = nextLyric.StartBeat;

        // Switch phrase 1/4 between the end of the current one and the start of the next one
        var switchBeat = endBeat + (endBeat - startBeat) / 4.0;

        if (!(beat >= switchBeat)) return;

        showLyric(nextLyric);
    }

    private AudioTrack? loadAudio(string directoryPath, string fileName)
    {
        try
        {
            var storage = new NativeStorage(directoryPath, host);
            using var activeAudioResourceStore = new StorageBackedResourceStore(storage);
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
        if (beat <= lastReceivedNoteBeat) return;

        notesContainer.AddSungNote(sungNote);
        lastReceivedNoteBeat = sungNote.StartBeat + sungNote.Length;
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);

        //CLEANUP PREVIOUS SONG TRACK & RESOURCES
        audioTrack?.Stop();
        audioTrack?.Dispose();
    }
}
