using System;
using osu.Framework.Graphics;
using osu.Framework.Graphics.UserInterface;
using osuTK;

namespace megastar.Game.presets;

public partial class BackButton : BasicButton
{
    /// <summary>
    /// A reusable back button with predefined styling.
    /// </summary>
    /// <param name="action">The method to execute when the button is clicked.</param>
    public BackButton(Action action, string text)
    {
        Text = text;
        Size = new Vector2(100, 40);

        // Setting default anchors, though these can still be overridden when instantiating
        Anchor = Anchor.TopLeft;
        Origin = Anchor.TopLeft;
        Position = new Vector2(10, 10);

        Action = action;
    }
}
