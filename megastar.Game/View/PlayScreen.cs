using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using megastar.Game.notes;
using megastar.Game.Preset;
using megastar.Game.Track;
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
    private osu.Framework.Timing.FramedClock videoClock;


    //The offset from the start of the screen where notes beginn to spawn
    private static float START_OFFSET = 1000f;

    private static AudioManager audioManager;
    private UsdxTrack curTrack;

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

    [BackgroundDependencyLoader]
    private void load(AudioManager audio)
    {
        audioManager = audio;

        InternalChildren = new Drawable[]
        {
            new Box
            {
                Colour = Color4.Violet,
                RelativeSizeAxes = Axes.Both,
            },
            backgroundLayer,
            new BackButton(this.Exit, "Go Back"),
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
                Text = "Error, Song konnte nicht geladen werden",
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
        notesContainer.Children = curNotes.Select(note => note.Visual).ToArray();

        track = loadSong(audioManager, usdxTrack.TrackMetadata.DirPath, usdxTrack.TrackMetadata.SongFile);
        track?.Start();

        loadBackgroundVideo(usdxTrack);
        //loadBackgroundImage(usdxTrack);
        curTrack = usdxTrack;
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
                    Video backgroundVideo = new Video(videoPath)
                    {
                        RelativeSizeAxes = Axes.Both,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        FillMode = FillMode.Fill,
                        Alpha = 0,
                        Loop = false,
                    };
                    osu.Framework.Timing.IClock sourceClock = track;


                    if (usdxTrack.TrackMetadata.VideoGap.IsNotNull())
                    {
                        sourceClock = new osu.Framework.Timing.OffsetClock(sourceClock)
                        {
                            Offset = usdxTrack.TrackMetadata.VideoGap
                        };
                    }


                    backgroundLayer.Add(backgroundVideo);
                    backgroundVideo.FadeIn(500, Easing.OutQuint);

                    //AddInternal(backgroundVideo);
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
        videoClock?.ProcessFrame();

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
            Logger.Error(ex, "Failed to load karaoke audio track.");
            return null;
        }
    }
}
