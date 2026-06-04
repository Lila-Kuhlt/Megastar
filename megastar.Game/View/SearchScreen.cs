using System;
using System.Linq;
using megastar.Game.Preset;
using megastar.Game.Track;
using megastar.Game.Translations;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Screens;
using osuTK;
using osuTK.Graphics;

namespace megastar.Game.View;

public partial class SearchScreen : Screen
{
    [Resolved] private MegastarGameBase game { get; set; } = null!;

    [BackgroundDependencyLoader]
    private void load()
    {
        var searchBox = new BasicTextBox
        {
            PlaceholderText = Fluent.Translate("search-query"),
            Size = new Vector2(400, 40),
            Anchor = Anchor.TopCentre,
            Origin = Anchor.TopCentre,
            Y = 50
        };

        InternalChildren =
        [
            new Box
            {
                Colour = StandardColours.BACKGROUND,
                RelativeSizeAxes = Axes.Both,
            },
            searchBox,
            new BackButton(this.Exit, Fluent.Translate("common-back")),

            new TrackCardScrollContainer(game.LoadedSongs.Select(trackData => new TrackCard(trackData.TrackMetadata)))
        ];
    }
}
