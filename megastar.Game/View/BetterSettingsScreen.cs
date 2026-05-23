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

public class Language
{
    public Language(string code, LocalisableString name, LocalisationManager localisationManager)
    {
        Code = code;
        Name = name;
        manager = localisationManager;
    }

    public string Code { get; }
    public LocalisableString Name { get; }
    private LocalisationManager manager;

    public override string ToString()
    {
        return manager.GetLocalisedString(Name);
    }
};

public partial class BetterSettingsScreen : Screen
{
    [Resolved] private MegastarGameBase game { get; set; } = null!;
    [Resolved] private FrameworkConfigManager config { get; set; }

    [BackgroundDependencyLoader]
    private void load(LocalisationManager localisation, List<string> locales)
    {
        List<Language> languages = new List<Language>();
        string savedLanguage = config.Get<string>(FrameworkSetting.Locale);
        Language initialLang = null;

        foreach (string lang in locales)
        {
            Language language = new Language(lang, Fluent.GetString(lang), localisation);
            languages.Add(language);
            if (lang == savedLanguage)
            {
                initialLang = language;
            }
        }

        BasicDropdown<Language> dropdown = new BasicDropdown<Language>
        {
            Anchor = Anchor.TopLeft,
            Origin = Anchor.TopLeft,
            Width = 200,
            Items = languages,
        };


        dropdown.Current.Value = initialLang ?? languages[0];

        dropdown.Current.ValueChanged += e =>
        {
            config.SetValue(FrameworkSetting.Locale, e.NewValue.Code);
            Logger.Log("[UPDATED LANGUAGE] " + e.NewValue.Code);
            dropdown.Items = languages;
        };


        InternalChildren =
        [
            new Box
            {
                Colour = Color4.Violet,
                RelativeSizeAxes = Axes.Both,
            },
            new BackButton(this.Exit, Fluent.GetString("common-back")),
            new SpriteText
            {
                Y = 20,
                Text = Fluent.GetString("main-settings"),
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

                Child = new FillFlowContainer()
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Direction = FillDirection.Vertical,
                    Spacing = new Vector2(0, 20),
                    Children = new Drawable[]
                    {
                        new StepSlider<int>("Volume", 0, 100, 100)
                        {
                            RelativeSizeAxes = Axes.X,
                            Width = 1f,
                            Height = 40,
                            Current = { BindTarget = Settings.GetSettings().SoundVolume }
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
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,
                                    Direction = FillDirection.Vertical,
                                    Spacing = new Vector2(0, 20),
                                    Width = 200,
                                    Children = new Drawable[]
                                    {
                                        // Spalte für Einstellungstexte
                                        new SpriteText
                                        {
                                            Text = Fluent.GetString("settings-language"),
                                            Margin = new MarginPadding(
                                                2), // Gleicher Margin wie der text im dropdown, sodass das zentriert ist
                                        },
                                        new SpriteText
                                        {
                                            Text = Fluent.GetString("settings-difficulty"),
                                            Margin = new MarginPadding(
                                                2), // Gleicher Margin wie der text im dropdown, sodass das zentriert ist
                                        }
                                    }
                                },
                                new FillFlowContainer()
                                {
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,
                                    Direction = FillDirection.Vertical,
                                    Spacing = new Vector2(0, 20), // Gap between buttons
                                    Children = new Drawable[]
                                    {
                                        // Spalte für Inputelemente
                                        dropdown,
                                        new BasicDropdown<GameDifficulty>
                                        {
                                            RelativeSizeAxes = Axes.X,
                                            Width = 1f,

                                            Items = System.Enum.GetValues<GameDifficulty>(),
                                            Current = { BindTarget = Settings.GetSettings().Difficulty }
                                        },
                                    }
                                }
                            }
                        }
                    }
                }
            }
        ];
    }
}
