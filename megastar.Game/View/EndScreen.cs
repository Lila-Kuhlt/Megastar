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
using osu.Framework.Graphics.Textures;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Screens;
using osuTK;
using osuTK.Graphics;

namespace megastar.Game.View
{
    public partial class EndScreen : Screen
    {
        [Resolved] private MegastarGameBase game { get; set; } = null!;

        private readonly Texture backgroundTexture;
        private int score;
        private int totalScore;
        private UsdxTrack lastTrack;

        public EndScreen(Texture backgroundTexture, UsdxTrack lastTrack, int score, int totalScore)
        {
            this.backgroundTexture = backgroundTexture;
            this.score = score;
            this.totalScore = totalScore;
            this.lastTrack = lastTrack;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            float performanceRatio = totalScore > 0 ? Math.Clamp((float)score / totalScore, 0f, 1f) : 0f;

            // Determine bar color based on performance
            Color4 performanceColor = performanceRatio > 0.8f ? Color4.LimeGreen :
                                      (performanceRatio > 0.5f ? Color4.Yellow : Color4.OrangeRed);

            InternalChildren = new Drawable[]
            {
                //Background Layer
                new ShaderBackground("sh_background.fs"),
                new Sprite
                {
                    RelativeSizeAxes = Axes.Both,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    FillMode = FillMode.Fill,
                    Texture = backgroundTexture,
                },

                // Makes the background darker so white UI text is highly readable
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Color4.Black,
                    Alpha = 0.65f
                },


                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Padding = new MarginPadding(50),
                    Children = new Drawable[]
                    {
                        // Last Song Metadata
                        new FillFlowContainer
                        {
                            Anchor = Anchor.TopLeft,
                            Origin = Anchor.TopLeft,
                            AutoSizeAxes = Axes.Both,
                            Direction = FillDirection.Vertical,
                            Children = new Drawable[]
                            {
                                new SpriteText
                                {
                                    Text = lastTrack?.TrackMetadata?.Title ?? "Unknown Title",
                                    Font = new FontUsage(size: 48, weight: "Bold"),
                                    Colour = Color4.White
                                },
                                new SpriteText
                                {
                                    Text = lastTrack?.TrackMetadata?.Artist ?? "Unknown Artist",
                                    Font = new FontUsage(size: 32),
                                    Colour = Color4.LightGray
                                }
                            }
                        },

                        //Left container
                        new FillFlowContainer
                        {
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                            RelativeSizeAxes = Axes.X,
                            AutoSizeAxes = Axes.Y,
                            Width = 0.45f,
                            Direction = FillDirection.Vertical,
                            Spacing = new Vector2(0, 15),
                            Children = new Drawable[]
                            {
                                new SpriteText
                                {
                                    Text = $"Score: {score} / {totalScore} ({(performanceRatio * 100):0.0}%)",
                                    Font = new FontUsage(size: 28, weight: "Bold")
                                },

                                new Container
                                {
                                    RelativeSizeAxes = Axes.X,
                                    Height = 30,
                                    Masking = true,
                                    CornerRadius = 15,
                                    Children = new Drawable[]
                                    {
                                        new Box
                                        {
                                            RelativeSizeAxes = Axes.Both,
                                            Colour = Color4.DarkGray,
                                            Alpha = 0.6f
                                        },
                                        new Box
                                        {
                                            RelativeSizeAxes = Axes.Both,
                                            Width = performanceRatio,
                                            Colour = performanceColor
                                        }
                                    }
                                },
                                new Container()
                                {
                                    //TODO Replace this with a nicer button once we have presets for them
                                    new BasicButton()
                                    {
                                        Text = $"Play Next Song : {game.PeekNextSong().TrackMetadata.Title} - {game.PeekNextSong().TrackMetadata.Artist}",
                                        Action = this.Exit,
                                        Size = new Vector2(500, 50)
                                    }
                                }
                            }
                        },

                        //TODO Right Side implement queue and search here
                        new Container
                        {
                            Anchor = Anchor.CentreRight,
                            Origin = Anchor.CentreRight,
                            RelativeSizeAxes = Axes.Both,
                            Width = 0.45f,
                            Children = new Drawable[]
                            {
                                new Box
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Colour = Color4.Black,
                                    Alpha = 0.4f,
                                },

                                new FillFlowContainer
                                {
                                    Anchor = Anchor.TopLeft,
                                    Origin = Anchor.TopLeft,
                                    RelativeSizeAxes = Axes.X,
                                    AutoSizeAxes = Axes.Y,
                                    Padding = new MarginPadding(20),
                                    Direction = FillDirection.Vertical,
                                    Spacing = new Vector2(0, 10),
                                    Children = new Drawable[]
                                    {
                                        new SpriteText { Text = Fluent.Translate("end-screen-add-queue"), Font = new FontUsage(size: 24, weight: "Bold") },
                                        //TODO this is a placeholder and should be replaced with the same logic as the final search
                                        new BasicTextBox
                                        {
                                            RelativeSizeAxes = Axes.X,
                                            Height = 40,
                                            PlaceholderText = "Search for an artist or song..."
                                        },
                                        new SpriteText
                                        {
                                            Text = Fluent.Translate("end-screen-next"),
                                            Font = new FontUsage(size: 24, weight: "Bold"),
                                            Margin = new MarginPadding { Top = 20 }
                                        },
                                    }
                                },

                                //TODO here Queue, the following is only placeholder
                                new BasicScrollContainer
                                {
                                    Anchor = Anchor.BottomLeft,
                                    Origin = Anchor.BottomLeft,
                                    RelativeSizeAxes = Axes.Both,
                                    // Pushes the top of the scroll container down so it doesn't overlap the search box
                                    Padding = new MarginPadding { Top = 160, Left = 20, Right = 20, Bottom = 20 },
                                    Child = new FillFlowContainer
                                    {
                                        RelativeSizeAxes = Axes.X,
                                        AutoSizeAxes = Axes.Y,
                                        Direction = FillDirection.Vertical,
                                        Spacing = new Vector2(0, 5),
                                        Children = game.QueuedSongs
                                            .Skip(1) // Ignores the first song in the list
                                            .Select((track, index) => new SpriteText
                                            {
                                                Text = $"{index + 1}. {track.TrackMetadata.Artist} - {track.TrackMetadata.Title}",
                                                Font = new FontUsage(size: 20),
                                                Colour = Color4.LightGray
                                            }).ToArray()
                                    }
                                }
                            }
                        }
                    }
                }
            };
        }
    }
}
