using megastar.Game.notes;
using megastar.Game.Preset;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Audio.Track;
using osu.Framework.Graphics;
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

    private UsdxNote note = new UsdxNote(1, 5, 5, "LOLOL", UsdxNoteType.Normal);
    private UsdxNote note2 = new UsdxNote(6, 5, 7, "LOLOL", UsdxNoteType.Normal);

    private UsdxNote[] curNotes = new UsdxNote[2];

    [BackgroundDependencyLoader]
    private void load(AudioManager audio)
    {
        track = loadSong(audio, @"C:\Users\jesko\RiderProjects\Megastar\megastar.Resources\Tracks", "Abba - Thank You For The Music.mp3");

        InternalChildren = new Drawable[]
        {
            //Background
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
            note.Visual,
            note2.Visual,
        };
    }

    public override void OnEntering(ScreenTransitionEvent e)
    {
        base.OnEntering(e);
        track?.Start();
    }

    protected override void Update()
    {
        base.Update();
        //Time in ms (for 60 fps = 16.66667)
        double delta = Time.Elapsed;
        note.Visual.X -= (float)delta * 0.5f;
    }

    //stop the track when leaving the screen so it doesn't leak into the menu
    public override bool OnExiting(ScreenExitEvent e)
    {
        track?.Stop();
        track?.Dispose();

        return base.OnExiting(e);
    }

    //FIXME hier könnte sehr viel overhead entstehen wenn das jedes mal neu geladen wird aber Turingmachine hat unendlich speicher
    private osu.Framework.Audio.Track.Track loadSong(AudioManager audioManager, string directoryPath, string fileName)
    {
        var storage = new NativeStorage(directoryPath);
        var resourceStore = new StorageBackedResourceStore(storage);
        ITrackStore customTrackStore = audioManager.GetTrackStore(resourceStore);
        return customTrackStore.Get(fileName);
    }
}
