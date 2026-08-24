using System;
using System.Collections.Generic;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Input;

namespace megastar.Game.Preset;

public sealed partial class TrackCardScrollContainer : BasicScrollContainer
{
    public TrackCardScrollContainer(IEnumerable<TrackCard> cards)
    {
        RelativeSizeAxes = Axes.Y;
        Width = 500;

        Anchor = Anchor.TopRight;
        Origin = Anchor.TopRight;

        Child = new FillFlowContainer
        {
            RelativeSizeAxes = Axes.X,
            AutoSizeAxes = Axes.Y,
            Direction = FillDirection.Vertical,

            //Spacing = new Vector2(0, 10),
            Spacing = Vector2.Zero,

            ChildrenEnumerable = cards
        };
    }
}
