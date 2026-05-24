using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osuTK;

public partial class SquircleButton : ClickableContainer
{
    private readonly Box background;
    private readonly SpriteIcon iconSprite;
    private readonly SpriteText textSprite;


    public Colour4 BackgroundColour
    {
        get => background.Colour;
        set => background.Colour = value;
    }

    public IconUsage Icon
    {
        get => iconSprite.Icon;
        set => iconSprite.Icon = value;
    }

    public string Text
    {
        get => textSprite.Text.ToString();
        set => textSprite.Text = value;
    }

    public SquircleButton()
    {
        Size = new Vector2(100);
        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;

        InternalChildren = new Drawable[]
        {
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Rotation = 45,
                Masking = true,
                CornerRadius = 25,
                Child = background = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Colour4.DeepPink
                }
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
                        Font = FrameworkFont.Regular.With(size: 16, weight: "Bold"),
                        Colour = Colour4.White,
                        Alpha = 0
                    }
                }
            }
        };
    }

    // Optional overrides to show/hide elements when values are assigned
    protected override void LoadComplete()
    {
        base.LoadComplete();
        if (!iconSprite.Icon.Equals(default(IconUsage))) iconSprite.Alpha = 1;
        if (!string.IsNullOrEmpty(textSprite.Text.ToString())) textSprite.Alpha = 1;
    }
}
