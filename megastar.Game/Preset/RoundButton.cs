using osu.Framework.Graphics;
using osu.Framework.Graphics.UserInterface;
using osuTK;

namespace megastar.Game.Preset;

public sealed partial class RoundButton : BasicButton
{
    /// <summary>
    /// A reusable back button with predefined styling.
    /// </summary>
    public RoundButton()
    {
        // Setting default anchors, though these can still be overridden when instantiating
        Anchor = Anchor.TopLeft;
        Origin = Anchor.TopLeft;
        Position = new Vector2(10, 10);

        Masking = true;
        CornerRadius = 20;

        BackgroundColour = StandardColours.MAIN;
    }
}
