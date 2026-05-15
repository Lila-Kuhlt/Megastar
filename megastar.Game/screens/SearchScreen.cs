using System;
using megastar.Game.presets;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Screens;
using osuTK;
using osuTK.Graphics;

namespace megastar.Game.screens;

public partial class SearchScreen : Screen
{
    [Resolved]
    private megastarGameBase game { get; set; } = null!;

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

        var searchContainer = new SearchContainer<UsdxTrack>
        {
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            AutoSizeAxes = Axes.Both,
            Direction = FillDirection.Vertical,
            Spacing = new Vector2(0, 10),

            Children = game.LoadedSongs.ToArray(),
        };

        //Bind the text changes to the search
        searchBox.Current.BindValueChanged(change =>
        {
            searchContainer.SearchTerm = change.NewValue;
        }, true);

        searchBox.OnCommit += (sender, isNew) =>
        {
            Console.WriteLine($"Search committed for: {sender.Text}");
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

            searchContainer
        };
    }
}
