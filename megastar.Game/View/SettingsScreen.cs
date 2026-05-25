using System.Collections.Generic;
using megastar.Game.Preset;
using megastar.Game.Translations;
using osu.Framework.Allocation;
using osu.Framework.Configuration;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Localisation;
using osu.Framework.Logging;
using osu.Framework.Screens;
using osu.Framework.Testing.Drawables.Steps;
using osuTK;
using osuTK.Graphics;

namespace megastar.Game.View;

public partial class SettingsScreen : Screen
{
    [Resolved] private MegastarGameBase game { get; set; } = null!;
    [Resolved] private FrameworkConfigManager config { get; set; }

    [BackgroundDependencyLoader]
    private void load(List<Language> locales, LocalisationManager localisation)
    {
        string savedLanguage = config.Get<string>(FrameworkSetting.Locale);
        Language initialLang = locales.Find((l) => l.Code == savedLanguage);

        BasicDropdown<Language> languageDropdown = new BasicDropdown<Language>
        {
            Anchor = Anchor.TopLeft,
            Origin = Anchor.TopLeft,
            Width = 200,
            Items = locales,
        };

        languageDropdown.Current.Value = initialLang ?? locales[0];

        languageDropdown.Current.ValueChanged += e =>
        {
            config.SetValue(FrameworkSetting.Locale, e.NewValue.Code);
            Logger.Log("[UPDATED LANGUAGE] " + e.NewValue.Code);
            languageDropdown.Items = locales;

        };


        InternalChildren =
        [
            new Box
            {
                Colour = Color4.Violet,
                RelativeSizeAxes = Axes.Both,
            },
            new BackButton(this.Exit, Fluent.Translate("common-back")),
            new SpriteText
            {
                Y = 20,
                Text = Fluent.Translate("main-settings"),
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

                Children = new Drawable[]
                {
                    new StepSlider<int>(localisation.GetLocalisedString(Fluent.Translate("settings-volume")), 0, 100, 100)
                    {
                        RelativeSizeAxes = Axes.X,
                        Width = 1f,
                        Height = 40,
                        Current = { BindTarget = Settings.GetSettings().SoundVolume },
                        Margin = new MarginPadding{Bottom = 80},
                    },
                    new FillFlowContainer()
                    {
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        Direction = FillDirection.Horizontal,
                        Children = new Drawable[]
                        {
                            new FillFlowContainer()
                            {
                                Anchor = Anchor.CentreLeft,
                                Origin = Anchor.CentreLeft,
                                Direction = FillDirection.Vertical,
                                Spacing = new Vector2(0, 20),
                                Width = 200,
                                Children = new Drawable[]
                                {
                                    // Spalte für Einstellungstexte
                                    new SpriteText
                                    {
                                        Text = Fluent.Translate("settings-language"),
                                        Padding = new MarginPadding { Vertical = 5 },
                                    },
                                    new SpriteText
                                    {
                                        Text = Fluent.Translate("settings-difficulty"),
                                        Padding = new MarginPadding { Vertical = 5 },
                                    }
                                }
                            },
                            new FillFlowContainer()
                            {
                                Anchor = Anchor.CentreLeft,
                                Origin = Anchor.CentreLeft,
                                Direction = FillDirection.Vertical,
                                Spacing = new Vector2(0, 20), // Gap between buttons
                                Children = new Drawable[]
                                {
                                    // Spalte für Inputelemente
                                    languageDropdown,
                                    new BasicDropdown<GameDifficulty>
                                    {
                                        Anchor = Anchor.TopLeft,
                                        Origin = Anchor.TopLeft,
                                        Width = 200,
                                        Items = System.Enum.GetValues<GameDifficulty>(),
                                        Current = { BindTarget = Settings.GetSettings().Difficulty }
                                    },
                                }
                            }
                        }
                    }
                }
            }
        ];
    }
}
