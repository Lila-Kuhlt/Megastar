using System;
using System.Linq;
using megastar.Game.Preset;
using megastar.Game.Track;
using megastar.Game.Translations;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Screens;
using osuTK;
using osuTK.Graphics;

namespace megastar.Game.View;

public partial class SearchScreen : Screen
{
    [Resolved] private MegastarGameBase game { get; set; } = null!;
    private FillFlowContainer queueContainer;
    private bool queueTwiceAllowed = true;

    [BackgroundDependencyLoader]
    private void load(MsTranslationStore t)
    {
        var searchBox = new BasicTextBox
        {
            PlaceholderText = t["search-query"],
            Size = new Vector2(400, 40),
            Anchor = Anchor.TopCentre,
            Origin = Anchor.TopCentre,
            Y = 50,
        };

        var searchContainer = new SearchContainer<UsdxTrackDrawable>
        {
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            AutoSizeAxes = Axes.Both,
            Direction = FillDirection.Vertical,
            Spacing = new Vector2(0, 10),
            X = -100,

            // Generates completely new UI objects every time the screen is entered
            Children = game.LoadedSongs.Select(trackData =>
                new UsdxTrackDrawable(trackData, track => AddToQueue(track))).ToArray(),
        };


        var queueBox = new SpriteText()
        {
            Text = t["Queue"],
            Size = new Vector2(400, 40),
            Anchor = Anchor.TopRight,
            Origin = Anchor.TopRight,
            Y = 50,
        };

        queueContainer = new FillFlowContainer()
        {
            Direction = FillDirection.Vertical,
            Spacing = new Vector2(0, 5),

            Width = 300,
            RelativeSizeAxes = Axes.Y,

            AutoSizeAxes = Axes.None,

            Anchor = Anchor.TopRight,
            Origin = Anchor.TopRight,
            Y = 100,
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


        InternalChildren =
        [
            new Box
            {
                Colour = Color4.Violet,
                RelativeSizeAxes = Axes.Both,
            },
            searchBox,
            queueBox,
            queueContainer,
            new BackButton(this.Exit, t["common-back"]),

            new BasicScrollContainer
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Size = new Vector2(500, 600),
                X = -100,
                Y = 30,

                Child = searchContainer
            },
        ];
    }

    //Quewe
    private void AddToQueue(UsdxTrack track)
    {
        if (!queueTwiceAllowed && MegastarGameBase.QueuedSongs.Contains(track))
        {
            AddInternal(new SpriteText()
            {
                Text = "Song was already added to the queue",
                Anchor = Anchor.BottomRight,
                Origin = Anchor.BottomRight,
            });
        }
        else
        {
            MegastarGameBase.QueuedSongs.Enqueue(track);
            queueContainer.Add(new SpriteText()
            {
                Text = $"{track.TrackMetadata.Title} - {track.TrackMetadata.Artist}"
            });


            AddInternal(new SpriteText()
            {
                Text = "Added to Queue",
                Anchor = Anchor.BottomCentre,
                Origin = Anchor.BottomCentre,
            });
        }
    }
}
