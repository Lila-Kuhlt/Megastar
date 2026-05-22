using Linguini.Bundle;
using megastar.Game.Preset;
using megastar.Game.Translations;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shaders;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Screens;
using osuTK;
using osuTK.Graphics;
using Veldrid;

namespace megastar.Game.View
{
    public partial class MainScreen : Screen
    {
        [BackgroundDependencyLoader]
        private void load(MsTranslationStore t)
        {
            InternalChildren =
            [
                //Background
                new ShaderBackground("sh_background.fs"),

                //Title
                new SpriteText
                {
                    Y = 20,
                    Text = t.GetAttrMessage("main-title"),
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
                            Text = t.GetAttrMessage("main-start-game"),
                            Size = new Vector2(200, 40),
                            BackgroundColour = Color4.Teal,
                            Action = () => this.Push(new PlayScreen()),
                        },
                        new BasicButton
                        {
                            Text = t.GetAttrMessage("main-search"),
                            Size = new Vector2(200, 40),
                            BackgroundColour = Color4.Teal,
                            Action = () => this.Push(new SearchScreen()),
                        },
                        new BasicButton
                        {
                            Text = t.GetAttrMessage("main-index"),
                            Size = new Vector2(200, 40),
                            BackgroundColour = Color4.Teal,
                            Action = () => this.Push(new FileSelectorScreen()),
                        },
                        new BackButton(Game.Exit, t.GetAttrMessage("main-exit"))
                    }
                }
            ];
        }
    }
}
