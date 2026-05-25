// We define the three states a word can be in

using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osuTK;

public enum LyricState
{
    Upcoming,
    Active,
    Passed
}

public partial class LyricWord : CompositeDrawable
{
    private LyricState currentState = LyricState.Upcoming;

    private readonly SpriteText mainText;
    private readonly SpriteText pulseText;

    public LyricWord(string text)
    {
        AutoSizeAxes = Axes.Both;
        Origin = Anchor.BottomCentre;
        Anchor = Anchor.BottomCentre;

        InternalChildren = new Drawable[]
        {
            //Only "effect"
            pulseText = new SpriteText
            {
                Text = text,
                Font = FontUsage.Default.With(size: 48, weight: "Bold"),
                Colour = Colour4.Gold,
                Anchor = Anchor.BottomCentre,
                Origin = Anchor.BottomCentre,
                Alpha = 0,
                AlwaysPresent = true,
                Blending = BlendingParameters.Additive
            },

            //Main Text
            mainText = new SpriteText
            {
                Text = text,
                Font = FontUsage.Default.With(size: 48, weight: "Bold"),
                Colour = Colour4.White,
                Anchor = Anchor.BottomCentre,
                Origin = Anchor.BottomCentre,
            }
        };
    }

    public void UpdateState(LyricState newState)
    {
        if (currentState == newState) return;
        currentState = newState;

        ClearTransforms(true);
        pulseText.ClearTransforms(true);

        switch (newState)
        {
            case LyricState.Upcoming:
                mainText.FadeColour(Colour4.White, 200, Easing.OutQuint);
                this.ScaleTo(1f, 200, Easing.OutQuint);

                pulseText.Alpha = 0;
                break;

            case LyricState.Active:
                mainText.FadeColour(Colour4.Gold, 150, Easing.OutQuint);
                this.ScaleTo(1.2f, 200, Easing.OutBack);

                pulseText.Scale = Vector2.One;
                pulseText.Alpha = 0.6f;
                pulseText.ScaleTo(1.6f, 400, Easing.OutQuint);
                pulseText.FadeOut(400, Easing.OutQuint);
                break;

            case LyricState.Passed:
                mainText.FadeColour(Colour4.Gray, 300, Easing.OutQuint);
                this.ScaleTo(1f, 300, Easing.OutQuint);

                pulseText.FadeOut(150, Easing.OutQuint);
                break;
        }
    }
}
