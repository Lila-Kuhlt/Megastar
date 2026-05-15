using megastar.Game.presets;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Screens;
using osuTK.Graphics;

namespace megastar.Game.screens;

public partial class PlayScreen : Screen
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
            new SpriteText
            {
                Text = "Hier könnten ihre Lyrics stehen",
                Anchor = Anchor.BottomCentre,
                Origin = Anchor.BottomCentre,
                Font = FontUsage.Default.With(size: 80),
            },
            new BackButton(this.Exit, "Go Back")
        };
    }
}
