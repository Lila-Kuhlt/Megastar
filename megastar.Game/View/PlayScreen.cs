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
    private osu.Framework.Audio.Track.Track track;

    [Resolved] private MegastarGameBase game { get; set; } = null!;
    [Resolved] private GameHost host { get; set; } = null!; // Resolved to manage NativeStorage instances safely

    private List<IBeatPaced> curNotes = new List<IBeatPaced>();


    //The offset from the start of the screen where notes beginn to spawn
    private static float START_OFFSET = 1000f;

    private static AudioManager audioManager;
    private UsdxTrack curTrack;
    private Video backgroundVideo;

    private Container notesContainer = new Container
    {
        AutoSizeAxes = Axes.Both,

        Anchor = Anchor.CentreLeft,
        Origin = Anchor.CentreLeft,

        AlwaysPresent = true
    };

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
    private MsTranslationStore t;

    [BackgroundDependencyLoader]
    private void load(AudioManager audio, MsTranslationStore msTranslationStore)
    {
        audioManager = audio;
        t = msTranslationStore;

        InternalChildren = new Drawable[]
        {
            new Box
            {
                Colour = Color4.Violet,
                RelativeSizeAxes = Axes.Both,
            },
            backgroundLayer,
            new BackButton(this.Exit, t["common-back"]),
            notesContainer
        };
    }

    public override void OnEntering(ScreenTransitionEvent e)
    {
        base.OnEntering(e);

        //TODO hier sollte irgendwie auch die nächsten Lieder abgespielt werden
        try
        {
            setUpTrack(game.QueuedSongs.First());
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
            AddInternal(new SpriteText()
            {
                Text = t["play-song-error"],
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
            });
        }
    }

    private void setUpTrack(UsdxTrack usdxTrack)
    {
        cleanUpOldStores();

        //Load notes
        curNotes = usdxTrack.Notes;
        List<List<IBeatPaced>> phrases = usdxTrack.NotePhrases;
        notesContainer.Children = curNotes.Select(note => note.Visual).ToArray();

        track = loadSong(audioManager, usdxTrack.TrackMetadata.DirPath, usdxTrack.TrackMetadata.SongFile);
        track?.Start();

        loadBackgroundImage(usdxTrack);
        loadBackgroundVideo(usdxTrack);
        curTrack = usdxTrack;
        track.Volume.Value = Settings.GetSettings().SoundVolume.Value / 100f;
    }

    private void cleanUpOldStores()
    {
        //CLEANUP PREVIOUS SONG TRACK & RESOURCES
        track?.Stop();
        track?.Dispose();
        track = null;
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
                    currentBackground.FadeIn(500, Easing.OutQuint);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, t["play-background-image-error"]);
            }
        }
    }

    private void loadBackgroundVideo(UsdxTrack usdxTrack)
    {
        if (usdxTrack.TrackMetadata.BackgroundVideoFile.IsNotNull())
        {
            try
            {
                string videoPath = Path.Combine(usdxTrack.TrackMetadata.DirPath, usdxTrack.TrackMetadata.BackgroundVideoFile);

                if (File.Exists(videoPath))
                {
                    // Let C# handle the file reading safely to bypass FFmpeg pathing issues
                    Stream videoStream = File.OpenRead(videoPath);

                    backgroundVideo = new Video(videoStream) // Pass the stream, not the string!
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

                    backgroundVideo.Clock = new osu.Framework.Timing.FramedOffsetClock(track)
                    {
                        Offset = gap
                    };

                    backgroundLayer.Add(backgroundVideo);

                    backgroundVideo.OnLoadComplete += v =>
                    {
                        Console.WriteLine("FERTIG GELADNE");
                        v.FadeIn(500, Easing.OutQuint);
                    };
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, t["play-background-video-error"]);
            }
        }
    }

    protected override void Update()
    {
        base.Update();

        if (curTrack != null && track != null)
        {
            double ultraStarBpm = curTrack.TrackMetadata.BPM;

            //track.CurrentTime should be in milliseconds.
            double currentBeat = ((track.CurrentTime - START_OFFSET - curTrack.TrackMetadata.Gap) / 60000.0) *
                                 ultraStarBpm * 4;
            notesContainer.X = (float)((-currentBeat * UsdxNote.SCALE_FACTOR));
        }
    }

    public override bool OnExiting(ScreenExitEvent e)
    {
        track?.Stop();
        track?.Dispose();
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
            Logger.Error(ex, t["play-audio-error"]);
            return null;
        }
    }
}
