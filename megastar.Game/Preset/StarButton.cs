using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osuTK;

public partial class StarButton : ClickableContainer
{
    private readonly SpriteIcon starBackground;
    private readonly SpriteIcon iconSprite;
    private readonly SpriteText textSprite;


    public Colour4 BackgroundColour
    {
        get => starBackground.Colour;
        set => starBackground.Colour = value;
    }

    public IconUsage Icon
    {
        get => iconSprite.Icon;
        set => iconSprite.Icon = value;
    }

    public LocalisableString Text
    {
        get => textSprite.Text.ToString();
        set => textSprite.Text = value;
    }

    public StarButton()
    {
        Size = new Vector2(150);
        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;
        Rotation = -30;

        InternalChildren = new Drawable[]
        {
            starBackground = new SpriteIcon
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Icon = FontAwesome.Solid.Star,
                Colour = Colour4.Gold
            },

            new FillFlowContainer
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Direction = FillDirection.Vertical,
                AutoSizeAxes = Axes.Both,
                Spacing = new Vector2(0, 5),
                Children = new Drawable[]
                {
                    iconSprite = new SpriteIcon
                    {
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        Size = new Vector2(30),
                        Colour = Colour4.White,
                        Alpha = 0
                    },
                    textSprite = new SpriteText
                    {
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        Font = FrameworkFont.Regular.With(size: 18, weight: "Bold"),
                        Colour = Colour4.White,
                        Alpha = 0
                    }
                }
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        if (!iconSprite.Icon.Equals(default(IconUsage))) iconSprite.Alpha = 1;
        if (!string.IsNullOrEmpty(textSprite.Text.ToString())) textSprite.Alpha = 1;
    }
}
