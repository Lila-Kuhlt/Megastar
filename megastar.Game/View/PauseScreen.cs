using megastar.Game.Preset;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osu.Framework.Screens;
using osuTK;
using osuTK.Graphics;
using osuTK.Input;

namespace megastar.Game.View;

public partial class PauseScreen : Screen
{
    [Resolved] private MegastarGameBase game { get; set; } = null!;

    private SpriteText cur_play_text = new SpriteText()
    {
        Anchor = Anchor.BottomCentre,
        Origin = Anchor.BottomCentre,
        Y = - 13,
        Colour = StandardColours.TEXT,
    };

    [BackgroundDependencyLoader]
    private void load()
    {
        cur_play_text.Text = $"Coming up next : {game.GetFirstSong()?.TrackMetadata.Title}";
        InternalChildren = new Drawable[]
        {
            new ShaderBackground("sh_background.fs"),

            // Makes the background darker so white UI text is highly readable
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Color4.Black,
                Alpha = 0.55f,
            },
            new FillFlowContainer
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                AutoSizeAxes = Axes.Both,
                Direction = FillDirection.Vertical,
                Spacing = new Vector2(10, 10),
                Children = new Drawable[]
                {
                    new RoundButton()
                    {
                        Text = "Resume",
                        Size = new Vector2(300, 70),
                        Action = () => this.Exit()
                    },
                    new RoundButton()
                    {
                        Text = $"Skip Song",
                        Size = new Vector2(300, 70),
                        Action = () =>
                        {
                            game.NextSong();
                            cur_play_text.Text = $"Coming up next : {game.GetFirstSong()?.TrackMetadata.Title}";
                        }
                    },
                }
            },
            new Container()
            {
                Anchor = Anchor.BottomCentre,
                Origin = Anchor.BottomCentre,
                RelativeSizeAxes = Axes.X,
                Width = 1f,
                Height = 50,
                Y = -100,

                Masking = true,
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = StandardColours.BACKGROUND_TEXT,
                        Alpha = 0.55f
                    },

                    cur_play_text
                }
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
