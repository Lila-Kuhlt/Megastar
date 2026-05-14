using System;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Screens;
using osuTK;
using osuTK.Graphics;

namespace megastar.Game.screens;

public partial class SearchScreen : Screen
{
    [BackgroundDependencyLoader]
    private void load()
    {
        //Create the text box as a variable first so that it can get modified to subscribe to an event
        var searchBox = new BasicTextBox
        {
            PlaceholderText = "Enter your search query...",
            Size = new Vector2(400, 40),
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
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
            new BasicButton()
            {
                Text = "Go Back",
                Anchor = Anchor.TopLeft,
                Origin = Anchor.TopLeft,
                Size = new Vector2(40, 40),
                Action = () => this.Exit()
            }
        };
    }
}
