using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using megastar.Game.notes;
using megastar.Game.Preset;
using megastar.Game.Track;
using megastar.Game.Translations;
using NUnit.Framework;
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
using osuTK.Graphics;

namespace megastar.Game.View;

public partial class PlayScreen : Screen
{
    private osu.Framework.Audio.Track.Track audioTrack;

    [Resolved] private MegastarGameBase game { get; set; } = null!;
    [Resolved] private GameHost host { get; set; } = null!; // Resolved to manage NativeStorage instances safely

    private List<IBeatPaced> curNotes = new List<IBeatPaced>();

    //Phrasing on Lyrics
    private List<List<INote>> allPhrases;
    private int currentPhraseIndex = 0;
    private LyricsContainer currentLyricsContainer;

    private readonly Container lyricsLayer = new Container
    {
        RelativeSizeAxes = Axes.Both,
        Padding = new MarginPadding { Bottom = 50 }
    };

    private readonly Container notesLayer = new Container
    {
        RelativeSizeAxes = Axes.Both,
        Anchor = Anchor.Centre,
        Origin = Anchor.Centre,
        AlwaysPresent = true
    };

    private PhraseNotesContainer currentNotesContainer;


    //The offset from the start of the screen where notes beginn to spawn
    private static float START_OFFSET = 300f;

    private static AudioManager audioManager;
    private UsdxTrack curTrack;
    private Video backgroundVideo;

    private double currentBeat = 0f;


    // Dedicated layer to safely swap background sprites behind UI elements
    private readonly Container backgroundLayer = new Container
    {
        RelativeSizeAxes = Axes.Both
    };

    private Sprite currentBackground;
    private TextureStore activeTextureStore;
    private StorageBackedResourceStore activeTextureResourceStore;
    private StorageBackedResourceStore activeAudioResourceStore;
    private StorageBackedResourceStore activeVideoRessourceStore;
    private FluentTranslationStore t;

    private uint lastReceivedNoteBeat = 0;

    [BackgroundDependencyLoader]
    private void load(AudioManager audio)
    {
        audioManager = audio;

        InternalChildren = new Drawable[]
        {
            new Box
            {
                Colour = StandardColours.BACKGROUND,
                RelativeSizeAxes = Axes.Both,
            },
            backgroundLayer,
            new BackButton(this.Exit, Fluent.Translate("common-back")),
            notesLayer,
            lyricsLayer
        };
    }

    public override void OnEntering(ScreenTransitionEvent e)
    {
        base.OnEntering(e);

        //TODO hier sollte irgendwie auch die nächsten Lieder abgespielt werden
        try
        {
            setUpTrack(game.NextSong());
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
            AddInternal(new SpriteText()
            {
                Text = Fluent.Translate("play-song-error"),
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
            });
        }
    }

    private void setUpTrack(UsdxTrack usdxTrack)
    {
        cleanUpOldStores();

        curNotes = usdxTrack.Notes;
        allPhrases = usdxTrack.NotePhrases;

        audioTrack = loadSong(audioManager, usdxTrack.TrackMetadata.DirPath, usdxTrack.TrackMetadata.SongFile);
        audioTrack?.Start();

        loadBackgroundImage(usdxTrack);
        loadBackgroundVideo(usdxTrack);
        curTrack = usdxTrack;
        audioTrack.Volume.Value = Settings.GetSettings().SoundVolume.Value / 100f;

        // first phrase
        currentPhraseIndex = 0;
        if (allPhrases != null && allPhrases.Count > 0)
        {
            showPhrase(currentPhraseIndex);
        }
    }


    private void showPhrase(int index)
    {
        lyricsLayer.Clear();
        notesLayer.Clear();

        var currentPhrase = allPhrases[index];

        currentLyricsContainer = new LyricsContainer(currentPhrase)
        {
            Anchor = Anchor.BottomCentre,
            Origin = Anchor.BottomCentre
        };
        lyricsLayer.Add(currentLyricsContainer);

        currentNotesContainer = new PhraseNotesContainer(currentPhrase);
        notesLayer.Add(currentNotesContainer);
    }

    private void cleanUpOldStores()
    {
        //CLEANUP PREVIOUS SONG TRACK & RESOURCES
        audioTrack?.Stop();
        audioTrack?.Dispose();
        audioTrack = null;
        activeAudioResourceStore?.Dispose();
        activeAudioResourceStore = null;
        activeVideoRessourceStore?.Dispose();
        activeVideoRessourceStore = null;

        // CLEANUP PREVIOUS BACKGROUND IMAGES & TEXTURE CACHES
        currentBackground?.Expire();
        backgroundLayer.Clear();
        activeTextureStore?.Dispose();
        activeTextureStore = null;
        activeTextureResourceStore?.Dispose();
        activeTextureResourceStore = null;
    }

    private void loadBackgroundImage(UsdxTrack usdxTrack)
    {
        if (usdxTrack.TrackMetadata.BackgroundImageFile.IsNotNull())
        {
            try
            {
                // Create clean virtual storage handles targetting the song's directory
                var textureStorage = new NativeStorage(usdxTrack.TrackMetadata.DirPath, host);
                activeTextureResourceStore = new StorageBackedResourceStore(textureStorage);
                activeTextureStore = new TextureStore(host.Renderer,
                    host.CreateTextureLoaderStore(activeTextureResourceStore));

                var texture = activeTextureStore.Get(usdxTrack.TrackMetadata.BackgroundImageFile);

                if (texture != null)
                {
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
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Failed to load karaoke track background image.");
            }
        }
    }

    private void loadBackgroundVideo(UsdxTrack usdxTrack)
    {
        if (usdxTrack.TrackMetadata.BackgroundVideoFile.IsNotNull())
        {
            try
            {
                string videoPath = Path.Combine(usdxTrack.TrackMetadata.DirPath,
                    usdxTrack.TrackMetadata.BackgroundVideoFile);

                if (File.Exists(videoPath))
                {
                    // Let C# handle the file reading safely to bypass FFmpeg pathing issues
                    Stream videoStream = File.OpenRead(videoPath);

                    backgroundVideo = new Video(videoStream)
                    {
                        RelativeSizeAxes = Axes.Both,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        FillMode = FillMode.Fill,
                        Alpha = 0,
                        Loop = true,
                    };

                    double gap = usdxTrack.TrackMetadata.VideoGap.IsNotNull()
                        ? (double)usdxTrack.TrackMetadata.VideoGap
                        : 0;

                    backgroundVideo.Clock = new osu.Framework.Timing.FramedOffsetClock(audioTrack)
                    {
                        Offset = gap
                    };

                    backgroundLayer.Add(backgroundVideo);

                    backgroundVideo.OnLoadComplete += v =>
                    {
                        v.FadeIn(0, Easing.OutQuint);
                    };
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Failed to load karaoke track background video.");
            }
        }
    }

    protected override void Update()
    {
        base.Update();


        if (curTrack != null && audioTrack != null)
        {
            double ultraStarBpm = curTrack.TrackMetadata.BPM;
            currentBeat = ((audioTrack.CurrentTime - curTrack.TrackMetadata.Gap) / 60000.0) * ultraStarBpm * 4;
            currentNotesContainer?.UpdateBeat(currentBeat);


            if (currentLyricsContainer != null)
            {
                currentLyricsContainer.beatTime = currentBeat;
            }

            handlePhraseSwitching();
        }
        //TODO hier nur zu testzwecken bis wirklicher input eingelesen wird
        ReceiveSungNote(new UsdxNote((uint)currentBeat, Random.Shared.Next(1, 5), Random.Shared.Next(5, 20), "", UsdxNoteType.Sung));
    }

    private void handlePhraseSwitching()
    {
        if (allPhrases != null && currentPhraseIndex + 1 < allPhrases.Count)
        {
            var currentPhrase = allPhrases[currentPhraseIndex];
            var nextPhrase = allPhrases[currentPhraseIndex + 1];

            if (currentPhrase.Count > 0 && nextPhrase.Count > 0)
            {
                var lastNote = currentPhrase.Last();
                var nextNote = nextPhrase.First();

                double phraseEndBeat = lastNote.StartBeat + lastNote.Length;
                double nextPhraseStartBeat = nextNote.StartBeat;

                // Switch phrase 1/4 between the end of the current one and the start of the next one
                double switchBeat = phraseEndBeat + ((nextPhraseStartBeat - phraseEndBeat) / 4.0);

                if (currentBeat >= switchBeat)
                {
                    currentPhraseIndex++;
                    showPhrase(currentPhraseIndex);
                }
            }
        }
    }

    public override bool OnExiting(ScreenExitEvent e)
    {
        audioTrack?.Stop();
        audioTrack?.Dispose();
        activeAudioResourceStore?.Dispose();

        activeTextureStore?.Dispose();
        activeTextureResourceStore?.Dispose();

        return base.OnExiting(e);
    }

    private osu.Framework.Audio.Track.Track loadSong(AudioManager audioManager, string directoryPath, string fileName)
    {
        try
        {
            var storage = new NativeStorage(directoryPath, host);
            activeAudioResourceStore = new StorageBackedResourceStore(storage);
            ITrackStore customTrackStore = audioManager.GetTrackStore(activeAudioResourceStore);
            return customTrackStore.Get(fileName);
        }
        catch (Exception ex)
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
        if ((uint) currentBeat > lastReceivedNoteBeat)
        {
            currentNotesContainer?.AddSungNote(sungNote);
            lastReceivedNoteBeat = sungNote.StartBeat + (uint) sungNote.Length;
        }
    }

    /// <summary>
    /// Returns the Beat at which the song currently is. New Input Notes should be set to this beat
    /// </summary>
    /// <returns></returns>
    public double GetCurrentBeat()
    {
        return currentBeat;
    }
}
