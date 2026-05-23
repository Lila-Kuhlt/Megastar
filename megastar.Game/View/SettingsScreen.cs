using megastar.Game.Preset;
using megastar.Game.Translations;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Screens;
using osu.Framework.Testing.Drawables.Steps;
using osuTK;
using osuTK.Graphics;

namespace megastar.Game.View
{
    public partial class SettingsScreen : Screen
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
                    Text = t["main-settings"],
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre,
                    Font = FontUsage.Default.With(size: 80),
                    Colour = Color4.Purple,
                    Shadow = true,
                    ShadowColour = Color4.Pink,
                },

                new BasicScrollContainer
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    RelativeSizeAxes = Axes.Both,
                    Width = 0.6f,
                    Height = 0.7f,

                    Child = new FillFlowContainer
                    {
                        RelativeSizeAxes = Axes.X,
                        Width = 1f,

                        AutoSizeAxes = Axes.Y,

                        Direction = FillDirection.Vertical,
                        Spacing = new Vector2(0, 15),

                        Children = new Drawable[]
                        {
                            new StepSlider<int>("Volume", 0, 100, 100)
                            {
                                RelativeSizeAxes = Axes.X,
                                Width = 1f,
                                Height = 40,
                                Current = { BindTarget = Settings.GetSettings().SoundVolume }
                            },
                            new BasicDropdown<GameDifficulty>
                            {
                                RelativeSizeAxes = Axes.X,
                                Width = 1f,

                                Items = System.Enum.GetValues<GameDifficulty>(),
                                Current = { BindTarget = Settings.GetSettings().Difficulty }
                            },
                            new BasicDropdown<string>
                            {
                                RelativeSizeAxes = Axes.X,
                                Width = 1f,

                                Items = t.GetAvailableResources(),

                                Current = { BindTarget = Settings.GetSettings().Language }
                            }
                        }
                    }
                },

                new BackButton(this.Exit, t["common-back"])
            ];
        }
    }
}
