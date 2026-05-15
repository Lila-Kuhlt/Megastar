using System;
using System.Linq; // Required for .Select()
using megastar.Game.presets;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers; // Required for FillFlowContainer
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites; // Required for SpriteText
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Screens;
using osuTK;
using osuTK.Graphics;

namespace megastar.Game.screens;

public partial class SearchScreen : Screen
{
    [Resolved]
    private MegastarGameBase game { get; set; } = null!;

    [BackgroundDependencyLoader]
    private void load()
    {
        var searchBox = new BasicTextBox
        {
            PlaceholderText = "Enter your search query...",
            Size = new Vector2(400, 40),
            Anchor = Anchor.TopCentre,
            Origin = Anchor.TopCentre,
            Y = 50
        };

        searchBox.OnCommit += (sender, isNew) =>
        {
            Console.WriteLine($"Search for: {sender.Text}");
        };

        InternalChildren = new Drawable[]
        {
            new Box
            {
                Colour = Color4.Violet,
                RelativeSizeAxes = Axes.Both,
            },
            searchBox,
            new BackButton(this.Exit, "Go Back"),

            // Container to cleanly stack the loaded songs vertically
            new FillFlowContainer
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                AutoSizeAxes = Axes.Both,
                Direction = FillDirection.Vertical,
                Spacing = new Vector2(0, 10),
                Children = game.LoadedSongs.Select(track => new SpriteText
                {
                    Text = track.trackMetadata.title + " - " + track.trackMetadata.artist,
                    Font = FontUsage.Default.With(size: 20),
                    Colour = Color4.White
                }).ToArray()
            }
        };
    }
}
