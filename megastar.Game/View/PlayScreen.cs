using System;
using System.Collections.Generic;
using System.Linq;
using megastar.Game.notes;
using megastar.Game.Preset;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Audio.Track;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.IO.Stores;
using osu.Framework.Platform;
using osu.Framework.Screens;
using osuTK.Graphics;

namespace megastar.Game.View;

public partial class PlayScreen : Screen
{
    private osu.Framework.Audio.Track.Track track;
    [Resolved] private MegastarGameBase game { get; set; } = null!;

    private List<IBeatPaced> curNotes = new List<IBeatPaced>();
    //TODO entfernen, sobald songs mit audio abgespielt werden können
    private double curTime = 0.0f;

    private Container notesContainer = new Container
    {
        RelativeSizeAxes = Axes.X,
        AutoSizeAxes = Axes.Y,
        Anchor = Anchor.CentreLeft,
        Origin = Anchor.CentreLeft,

        AlwaysPresent = true
    };

    [BackgroundDependencyLoader]
    private void load(AudioManager audio)
    {
        track = loadSong(audio, @"C:\Users\jesko\RiderProjects\Megastar\megastar.Resources\Tracks",
            "Abba - Thank You For The Music.mp3");

        InternalChildren = new Drawable[]
        {
            new Box
            {
                Colour = Color4.Violet,
                RelativeSizeAxes = Axes.Both,
            },
            new SpriteText
            {
                Text = "Hier könnten ihre Lyrics stehen",
                Anchor = Anchor.BottomCentre,
                Origin = Anchor.BottomCentre,
                Font = FontUsage.Default.With(size: 80),
            },
            new BackButton(this.Exit, "Go Back"),

            notesContainer = new Container
            {
                RelativeSizeAxes = Axes.Both,
            }
        };
    }

    public override void OnEntering(ScreenTransitionEvent e)
    {
        base.OnEntering(e);
        track?.Start();

        //TODO hier sollte irgendwie auch die nächsten Lieder abgespielt werden
        curNotes = game.QueuedSongs.First().Notes;
        notesContainer.Children = curNotes.Select(note => note.Visual).ToArray();
    }

    protected override void Update()
    {
        base.Update();
        double ultraStarBpm = game.QueuedSongs.First().TrackMetadata.BPM;


        curTime += Time.Elapsed;

        //TODO hier muss der Track dann angeschlossen werden
        //track.CurrentTime should be in milliseconds.
        //double currentBeat = ((track?.CurrentTime) / 60000.0) * ultraStarBpm;
        double currentBeat = (curTime / 60000.0) * ultraStarBpm * 4;
        notesContainer.X = (float)(-currentBeat * UsdxNote.SCALE_FACTOR);
    }


    public override bool OnExiting(ScreenExitEvent e)
    {
        track?.Stop();
        track?.Dispose();

        return base.OnExiting(e);
    }

    private osu.Framework.Audio.Track.Track loadSong(AudioManager audioManager, string directoryPath, string fileName)
    {
        var storage = new NativeStorage(directoryPath);
        var resourceStore = new StorageBackedResourceStore(storage);
        ITrackStore customTrackStore = audioManager.GetTrackStore(resourceStore);
        return customTrackStore.Get(fileName);
    }
}
