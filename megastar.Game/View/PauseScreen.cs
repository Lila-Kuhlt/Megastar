using System;
using System.Linq;
using Linguini.Shared.Types.Bundle;
using megastar.Game.Preset;
using megastar.Game.Track;
using megastar.Game.Translations;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;
using osu.Framework.Screens;
using osuTK;
using osuTK.Graphics;
using osuTK.Input;

namespace megastar.Game.View;

public partial class PauseScreen : Screen
{
    [Resolved] private MegastarGameBase game { get; set; } = null!;


    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChildren = new Drawable[]
        {
            // Makes the background darker so white UI text is highly readable
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Color4.Black,
                Alpha = 0.65f
            },
            new SpriteText()
            {
                Size = new Vector2(100, 30),
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Text = "LOLOLOL"
            }
        };
    }

    protected override bool OnKeyDown(KeyDownEvent e)
    {
        if (e.Key == Key.Escape)
        {
            this.Exit();
        }
        return base.OnKeyDown(e);
    }
}


