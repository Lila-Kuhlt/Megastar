using System.Collections.Generic;
using System.Linq;
using megastar.Game.notes;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace megastar.Game.Preset;

public partial class PhraseNotesContainer : Container
{
    private readonly Container targetNotesLayer;
    private readonly Container sungNotesLayer;
    private readonly Box playhead;

    private readonly float offsetX;
    private readonly float offsetY;

    public PhraseNotesContainer(List<INote> phraseNotes)
    {
        RelativeSizeAxes = Axes.Both;
        targetNotesLayer = new Container
        {
            Anchor = Anchor.CentreLeft,
            Origin = Anchor.CentreLeft,
            AutoSizeAxes = Axes.Both
        };

        sungNotesLayer = new Container
        {
            Anchor = Anchor.CentreLeft,
            Origin = Anchor.CentreLeft,
            AutoSizeAxes = Axes.Both
        };

        playhead = new Box
        {
            Width = 4,
            RelativeSizeAxes = Axes.Y,
            Colour = Colour4.White.Opacity(0.5f),
            Origin = Anchor.TopCentre,
            Depth = -1
        };

        AddInternal(targetNotesLayer);
        AddInternal(sungNotesLayer);
        AddInternal(playhead);

        if (phraseNotes.Count > 0)
        {
            // Pins the first note 230px from the left edge
            uint phraseStartBeat = phraseNotes[0].StartBeat;
            offsetX = -phraseStartBeat * UsdxNote.SCALE_FACTOR + 230f;

            float totalPitch = 0;
            int noteCount = 0;

            foreach (var beat in phraseNotes)
            {
                if (beat is UsdxNote usdxNote)
                {
                    totalPitch += usdxNote.Pitch;
                    noteCount++;
                }
            }

            float averagePitch = noteCount > 0 ? totalPitch / noteCount : 0;


            offsetY = averagePitch * UsdxNote.HEIGHT_FACTOR;

            targetNotesLayer.X = offsetX;
            targetNotesLayer.Y = offsetY;

            sungNotesLayer.X = offsetX;
            sungNotesLayer.Y = offsetY;

            foreach (var note in phraseNotes)
            {
                targetNotesLayer.Add(note.Visual);
            }
        }
    }

    public void UpdateBeat(double currentBeat)
    {
        playhead.X = (float)(currentBeat * UsdxNote.SCALE_FACTOR) + offsetX;
    }

    public void AddSungNote(INote sungNote)
    {
        sungNotesLayer.Add(sungNote.Visual);
    }
}
