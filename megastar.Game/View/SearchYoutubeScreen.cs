using System;
using System.Collections.Generic;
using System.IO;
using megastar.Game.Preset;
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
using YoutubeExplode.Videos.Streams;

namespace megastar.Game.View;

public partial class SearchYoutubeScreen : Screen
{
    private static int VIDEOS_TO_SEARCH = 10;
    private YoutubeClient youtube = new YoutubeClient();
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
        var topVideos = await youtube.Search.GetVideosAsync(searchTerm).CollectAsync(VIDEOS_TO_SEARCH);
        List<VideoSearchResult> results = new List<VideoSearchResult>();
        foreach (var video in topVideos)
        {
            results.Add(video);
            Console.WriteLine(video.Title);
        }

        var newChildDrawables = new Drawable[results.Count];
        Console.WriteLine(results);


        for (int i = 0; i < results.Count; i++)
        {
            // FIX THE SILENT BUG: Capture the current video in a local variable
            var video = results[i];

            newChildDrawables[i] = new FillFlowContainer()
            {
                Direction = FillDirection.Horizontal,
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Spacing = new Vector2(15, 0),

                Children = new Drawable[]
                {
                    new SpriteText()
                    {
                        Text = video.Title,
                        Width = 450,
                        Truncate = true,
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                    },
                    new BasicButton()
                    {
                        Text = "Download",
                        Size = new Vector2(120, 40),
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        Action = () => downloadSong(video)
                    }
                }
            };
        }

        songContainer.Children = newChildDrawables;
    }

    private async void downloadSong(VideoSearchResult video)
    {
        var streamManifest = await youtube.Videos.Streams.GetManifestAsync(video.Url);
        var audioStream = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();

        Settings settings = Settings.GetSettings();
        //TODO Fix this to set a custom path
        string songPath = settings.LastIndexPath.Value;


        await youtube.Videos.Streams.DownloadAsync(audioStream, Path.Combine(songPath, $"{video.Title}.mp3"));
    }
}
