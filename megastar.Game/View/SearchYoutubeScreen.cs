using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
using YoutubeExplode;
using YoutubeExplode.Common;
using YoutubeExplode.Search;
using YoutubeExplode.Videos;

namespace megastar.Game.View;

public partial class SearchYoutubeScreen : Screen
{
    private YoutubeClient youtube = new YoutubeClient();
    [Resolved] private MegastarGameBase game { get; set; } = null!;
    private FillFlowContainer songContainer;

    [BackgroundDependencyLoader]
    private void load()
    {
        var searchBox = new BasicTextBox
        {
            PlaceholderText = Fluent.Translate("search-query"),
            Size = new Vector2(400, 40),
            Anchor = Anchor.TopCentre,
            Origin = Anchor.TopCentre,
            Y = 50,
        };

        songContainer = new FillFlowContainer()
        {
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            AutoSizeAxes = Axes.Both,
            Direction = FillDirection.Vertical,
            Spacing = new Vector2(0, 10),
            X = -100,
        };


        searchBox.OnCommit += (sender, isNew) =>
        {
            Schedule(searchForSong, sender.Text);
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
            new BackButton(this.Exit, Fluent.Translate("common-back")),

            new BasicScrollContainer
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Size = new Vector2(1000, 600),
                X = -100,
                Y = 30,

                Child = songContainer
            }
        ];
    }

    private async void searchForSong(string searchTerm)
    {
        //var vid = await youtube.Videos.GetAsync(searchTerm);
        //Console.WriteLine(vid.Title);
        //Console.WriteLine(vid.Author);
        var topVideos = await youtube.Search.GetVideosAsync(searchTerm).CollectAsync(10);
        List<VideoSearchResult> results = new List<VideoSearchResult>();
        foreach (var video in topVideos)
        {
            results.Add(video);
            var id = video.Id;
            var title = video.Title;
            var duration = video.Duration;
        }

        var newChildDrawables = new SpriteText[results.Count];
        Console.WriteLine(results);

        foreach (var result in results)
        {
            Console.WriteLine(result.Title);
        }


        for (int i = 0; i < 10; i++)
        {
            newChildDrawables[i] = new SpriteText()
            {
                Text = results[i].Title,
                Size = new Vector2(800, 40),
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
            };
        }

        songContainer.Children =  newChildDrawables;
    }
}
