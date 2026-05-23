using JetBrains.Annotations;
using megastar.Game.Track;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Transforms;
using osu.Framework.Input.Events;
using osu.Framework.Logging;
using osuTK;

namespace megastar.Game.Preset;

public sealed partial class TrackCard : Container
{
    private float baseWidth = 400;

    public TrackCard(ITrackMetadata metadata)
    {
        Name = $"\"{metadata.Artist} - {metadata.Title}\" TrackCard";

        Height = 80;
        Width = baseWidth;

        CornerRadius = 10;
        Masking = true;

        Padding = new MarginPadding(2);

        Anchor = Anchor.CentreRight;
        Origin = Anchor.TopRight;

        Shear = new Vector2(0.1f, 0.0f);

        Children =
        [
            new Box { RelativeSizeAxes = Axes.Both, Colour = Colour4.Black.Opacity(0.5f) },
            new FillFlowContainer
            {
                Direction = FillDirection.Vertical,
                AutoSizeAxes = Axes.Both,
                Spacing = Vector2.UnitY,
                Padding = new MarginPadding(10),
                Children =
                [
                    new SpriteText { Text = metadata.Title, Font = new FontUsage("Roboto", weight: "bold", size: 24) },
                    new SpriteText { Text = metadata.Artist, Font = new FontUsage("Roboto", size: 18) },
                ],
            }
        ];
    }

    protected override bool OnHover(HoverEvent e)
    {
        this.ResizeWidthTo(baseWidth * 1.1f, 125.0f, Easing.InOutQuint);
        return base.OnHover(e);
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        base.OnHoverLost(e);
        this.ResizeWidthTo(baseWidth, 125.0f, Easing.InOutQuint)
            .ScaleTo(1f, 125f, Easing.InOutQuint);
    }
}
