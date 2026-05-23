// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osuTK.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;

namespace megastar.Game.Preset
{
    /// <summary>
    /// An almost exact copy of the <see cref="BasicButton"/> with the difference being, this one automatically resizes the x-axis to fit the text. (This e.g. prevents translations with different lengths to break out of the button box.)
    /// </summary>
    public partial class TextFitButton : Button
    {
        public LocalisableString Text
        {
            get => SpriteText.Text;
            set => SpriteText.Text = value;
        }

        public Color4 BackgroundColour
        {
            get => Background.Colour;
            set => Background.FadeColour(value);
        }

        private Color4? flashColour;

        /// <summary>
        /// The colour the background will flash with when this button is clicked.
        /// </summary>
        public Color4 FlashColour
        {
            get => flashColour ?? BackgroundColour;
            set => flashColour = value;
        }

        /// <summary>
        /// The additive colour that is applied to the background when hovered.
        /// </summary>
        public Color4 HoverColour
        {
            get => Hover.Colour;
            set => Hover.FadeColour(value);
        }

        private Color4 disabledColour = Color4.Gray;

        /// <summary>
        /// The additive colour that is applied to this button when disabled.
        /// </summary>
        public Color4 DisabledColour
        {
            get => disabledColour;
            set
            {
                if (disabledColour == value)
                    return;

                disabledColour = value;
                Enabled.TriggerChange();
            }
        }

        /// <summary>
        /// The duration of the transition when hovering.
        /// </summary>
        public double HoverFadeDuration { get; set; } = 200;

        /// <summary>
        /// The duration of the flash when this button is clicked.
        /// </summary>
        public double FlashDuration { get; set; } = 200;

        /// <summary>
        /// The duration of the transition when toggling the Enabled state.
        /// </summary>
        public double DisabledFadeDuration { get; set; } = 200;

        private MarginPadding textPadding = new MarginPadding { Horizontal = 20 };

        /// <summary>
        /// The padding for the internal text (default is 20 horizontal)
        /// </summary>
        public MarginPadding TextPadding
        {
            get => textPadding;
            set
            {
                textPadding = value;
                if (SpriteText != null)
                    SpriteText.Margin = textPadding;
            }
        }

        protected Box Hover;
        protected Box Background;
        protected SpriteText SpriteText;

        public TextFitButton()
        {
            AutoSizeAxes = Axes.X;

            AddRange(new Drawable[]
            {
                Background = new Box
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    RelativeSizeAxes = Axes.Both,
                    Colour = FrameworkColour.BlueGreen
                },
                Hover = new Box
                {
                    Alpha = 0,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    RelativeSizeAxes = Axes.Both,
                    Colour = Color4.White.Opacity(.1f),
                    Blending = BlendingParameters.Additive
                },
                SpriteText = CreateText()
            });

            Enabled.BindValueChanged(enabledChanged, true);
        }

        protected virtual SpriteText CreateText() => new SpriteText
        {
            Depth = -1,
            Origin = Anchor.Centre,
            Anchor = Anchor.Centre,
            Font = FrameworkFont.Regular,
            Colour = FrameworkColour.Yellow,
            Margin = TextPadding
        };

        protected override bool OnClick(ClickEvent e)
        {
            if (Enabled.Value)
                Background.FlashColour(FlashColour, FlashDuration);

            return base.OnClick(e);
        }

        protected override bool OnHover(HoverEvent e)
        {
            if (Enabled.Value)
                Hover.FadeIn(HoverFadeDuration);

            return base.OnHover(e);
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            base.OnHoverLost(e);

            Hover.FadeOut(HoverFadeDuration);
        }

        private void enabledChanged(ValueChangedEvent<bool> e)
        {
            this.FadeColour(e.NewValue ? Color4.White : DisabledColour, DisabledFadeDuration, Easing.OutQuint);
        }
    }
}
