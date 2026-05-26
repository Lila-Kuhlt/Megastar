using megastar.Game.Preset;
using megastar.Game.Translations;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
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
            if (Settings.GetSettings().WebAppStart.Value)
            {
                AddInternal(game.LocalQueueServer);
            }
            InternalChildren =
            [
                //Background
                new ShaderBackground("sh_background.fs"),


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

                new FillFlowContainer
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    AutoSizeAxes = Axes.Both, // Shrink-wraps to fit the buttons
                    Direction = FillDirection.Vertical,
                    Spacing = new Vector2(0, 10), // Gap between buttons
                    Children = new Drawable[]
                    {
                        new BasicButton
                        {
                            Text = Fluent.Translate("main-start-game"),
                            Size = new Vector2(200, 40),
                            BackgroundColour = Color4.Teal,
                            Action = () => this.Push(new PlayScreen()),
                        },
                        new BasicButton
                        {
                            Text = Fluent.Translate("main-search"),
                            Size = new Vector2(200, 40),
                            BackgroundColour = Color4.Teal,
                            Action = () => this.Push(new SearchScreen()),
                        },
                        new BasicButton
                        {
                            Text = Fluent.Translate("main-index"),
                            Size = new Vector2(200, 40),
                            BackgroundColour = Color4.Teal,
                            Action = () => this.Push(new FileSelectorScreen()),
                        },
                        new BasicButton
                        {
                            Text = Fluent.Translate("main-settings"),
                            Size = new Vector2(200, 40),
                            BackgroundColour = Color4.Teal,
                            Action = () => this.Push(new SettingsScreen()),
                        },
                        new BackButton(Game.Exit, Fluent.Translate("main-exit"))
                    }
                }
            ];
        }
    }
}
