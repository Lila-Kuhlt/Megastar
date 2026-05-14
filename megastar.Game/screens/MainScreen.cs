using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Screens;
using osuTK;
using osuTK.Graphics;

namespace megastar.Game.screens
{
    public partial class MainScreen : Screen
    {
        [BackgroundDependencyLoader]
        private void load()
        {
            InternalChildren = new Drawable[]
            {
                //Background
                new Box
                {
                    Colour = Color4.Violet,
                    RelativeSizeAxes = Axes.Both,
                },
                //Title
                new SpriteText
                {
                    Y = 20,
                    Text = "ULTRA MEGA PREMIUM DELUXE KARAOKE",
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
                    AutoSizeAxes = Axes.Both, // Shrink-wraps to fit the buttons
                    Direction = FillDirection.Vertical,
                    Spacing = new Vector2(0, 10), // Gap between buttons
                    Children = new Drawable[]
                    {
                        new BasicButton
                        {
                            Text = "Start Game",
                            Size = new Vector2(200, 40),
                            BackgroundColour = Color4.Teal,
                            Action = () => this.Push(new PlayScreen()),
                        },
                        new BasicButton
                        {
                            Text = "Search",
                            Size = new Vector2(200, 40),
                            BackgroundColour = Color4.Teal,
                            Action = () => this.Push(new SearchScreen()),
                        },
                        new BasicButton()
                        {
                            Text = "Exit",
                            Size = new Vector2(200, 40),
                            BackgroundColour = Color4.Teal,
                            Action = () => Game?.Exit()
                        }
                    }
                }
            };
        }
    }
}
