using System.Collections.Generic;
using megastar.Game.notes;
using megastar.Game.Track;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace megastar.Game.Preset;

/// <summary>
/// A container for multiple <see cref="LyricWord"/>.
/// This automatically creates the corresponding <see cref="LyricWord"/>s and also updates their <see cref="LyricState"/> corresponding to the value of <code>beatTime</code>.
/// Therefore <code>beatTime</code> should always be kept up to date.
/// </summary>
public sealed partial class LyricsContainer : Container // Changed from FillFlowContainer to standard Container
{
    public double BeatTime { get; private set; }

    private readonly Lyric lyric;
    private readonly Dictionary<INote, LyricWord> wordDrawables = new Dictionary<INote, LyricWord>();

    public LyricsContainer(Lyric lyric)
    {
        this.lyric = lyric;
        RelativeSizeAxes = Axes.Both;

        Add(new Box()
        {
            Colour = Colour4.Black,
            RelativeSizeAxes = Axes.Both,
            Height = 0.1f,
            Anchor = Anchor.BottomLeft,
            Origin = Anchor.BottomLeft,
            Alpha = 0.5f
        });

        var flowContainer = new FillFlowContainer
        {
            AutoSizeAxes = Axes.Both,
            Direction = FillDirection.Horizontal,
            Spacing = new Vector2(10, 0),
            Anchor = Anchor.BottomCentre,
            Origin = Anchor.BottomCentre,
            Margin = new MarginPadding { Bottom = 15 }
        };
        Add(flowContainer);

        foreach (var beat in lyric.Notes)
        {
            string textToDisplay = beat.Text;

            var wordDrawable = new LyricWord(textToDisplay)
            {
                Margin = new MarginPadding { Horizontal = 1 }
            };

            wordDrawables[beat] = wordDrawable;
            flowContainer.Add(wordDrawable);
        }
    }

    public void UpdateBeat(double beat) => BeatTime = beat;

    protected override void Update()
    {
        base.Update();

        foreach (var beat in lyric.Notes)
        {
            var word = wordDrawables[beat];
            double endBeat = beat.StartBeat + beat.Length;

            if (BeatTime >= beat.StartBeat && BeatTime <= endBeat)
            {
                word.UpdateState(LyricState.Active);
            }
            else if (BeatTime > endBeat)
            {
                word.UpdateState(LyricState.Passed);
            }
            else
            {
                word.UpdateState(LyricState.Upcoming);
            }
        }
    }
}
