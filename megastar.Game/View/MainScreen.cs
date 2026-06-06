using megastar.Game.Preset;
using System.Linq;
using System.Net.Mime;
using megastar.Game.Translations;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.IO.Stores;
using osu.Framework.Screens;
using osuTK;
using osuTK.Graphics;

namespace megastar.Game.View
{
    public partial class MainScreen : Screen
    {
        [Resolved] private MegastarGameBase game { get; set; } = null!;

        [BackgroundDependencyLoader]
        private void load()
        {
            InternalChildren =
            [
                //Background
                new ShaderBackground("sh_background.fs"),

                //Always add Server to children, activate it via its start method
                game.LocalQueueServer,

                //Title
                new SpriteText
                {
                    Y = 20,
                    Text = Fluent.Translate("main-title"),
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre,
                    Font = FontUsage.Default.With(size: 80),
                    Colour = Color4.Purple,
                    Shadow = true,
                    ShadowColour = Color4.Pink,
                },

                new FillFlowContainer()
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    AutoSizeAxes = Axes.Both,
                    Direction = FillDirection.Horizontal,
                    Children = new Drawable[]
                    {
                        new StarButton()
                        {
                            Size = new Vector2(300),
                            Text = Fluent.Translate("main-title"),
                        },
                        new SquircleButton()
                        {
                            Text = Fluent.Translate("main-start-game"),
                            Action = () => this.Push(new PlayScreen()),
                            Size = new Vector2(150),
                            Depth = 1,
                            BackgroundColour = StandardColours.MAIN
                        },
                        new SquircleButton
                        {
                            Text = Fluent.Translate("main-search"),
                            Action = () => this.Push(new SearchScreen()),
                            Size = new Vector2(150),
                            Depth = 2,
                            BackgroundColour = StandardColours.SECOND
                        },
                        new SquircleButton
                        {
                            Text = Fluent.Translate("main-index"),
                            Action = () => this.Push(new FileSelectorScreen()),
                            Size = new Vector2(150),
                            Depth = 3,
                            BackgroundColour = StandardColours.THIRD
                        },
                        new SquircleButton
                        {
                            Text = Fluent.Translate("main-settings"),
                            BackgroundColour = StandardColours.MAIN,
                            Action = () => this.Push(new SettingsScreen()),
                            Size = new Vector2(150),
                            Depth = 4
                        },
                        new SquircleButton
                        {
                            Text = Fluent.Translate("main-exit"),
                            BackgroundColour = StandardColours.SECOND,
                            Action = () => Game.Exit(),
                            Size = new Vector2(150),
                            Depth = 5
                        },
                    }
                },
                new SpriteText()
                {
                    Text = FunFact.GetCowFunfact(),
                    Anchor = Anchor.BottomCentre,
                    Origin = Anchor.BottomCentre,
                    Font = new FontUsage("kuuhleFont", 40f),
                    Y = -35,
                    Truncate = true,
                }
            ];

            if (Settings.GetSettings().WebAppActive.Value && game.LocalQueueServer != null)
            {
                game.LocalQueueServer.StartWebserver();
            }
        }
    }
}
