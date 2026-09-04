using System;
using System.Collections.Generic;
using System.Linq;
using megastar.Game.notes;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace megastar.Game.Preset;

/// <summary>
/// This is a container for notes.
/// It displays visually based on their pitch and length
/// </summary>
public sealed partial class NoteContainer : Container
{
    private readonly Container targetNotesLayer;
    private readonly Container sungNotesLayer;
    private readonly Box playhead;

    private readonly float offsetY;
    private readonly float phraseStartOffset;
    private readonly float naturalPhraseWidth;

    private double currentBeat;

    /// <summary>
    /// This will instantiate a new container with the given Notes. It will only use the pitch and length to visualize
    /// </summary>
    /// <param name="notes">The Notes to display</param>
    public NoteContainer(List<INote> notes)
    {
        RelativeSizeAxes = Axes.Both;
        Padding = new MarginPadding { Horizontal = 200 };

        targetNotesLayer = new Container
        {
            Anchor = Anchor.CentreLeft,
            Origin = Anchor.CentreLeft,
            AutoSizeAxes = Axes.Both
        };

        //This used to be different layers but for scaling reasons, it is much simpler when they are the same. For potential future separation, this interface should still keep both layers separate.
        sungNotesLayer = targetNotesLayer;

        //This box indicates the current note to be sung by moving along with the beat
        playhead = new Box
        {
            Width = 4,
            RelativeSizeAxes = Axes.Y,
            Colour = Colour4.White.Opacity(0.5f),
            Origin = Anchor.TopCentre,
            Depth = -1
        };

        AddInternal(targetNotesLayer);
        AddInternal(playhead);

        if (notes.Count <= 0) return;

        //phrase boundaries
        float phraseStartBeat = notes[0].StartBeat;
        var lastNote = notes[^1];
        float phraseEndBeat = lastNote.StartBeat + lastNote.Length;

        //Define natural base dimensions before screen-scaling
        phraseStartOffset = phraseStartBeat * UsdxNote.SCALE_FACTOR;
        naturalPhraseWidth = (phraseEndBeat - phraseStartBeat) * UsdxNote.SCALE_FACTOR * 1.5f;

        float totalPitch = 0;
        int noteCount = 0;

        foreach (var beat in notes)
        {
            if (beat is not UsdxNote usdxNote) continue;

            totalPitch += usdxNote.Pitch;
            noteCount++;
        }

        float averagePitch = noteCount > 0 ? totalPitch / noteCount : 0;
        offsetY = averagePitch * UsdxNote.HEIGHT_FACTOR;

        targetNotesLayer.Y = offsetY;
        sungNotesLayer.Y = offsetY;

        foreach (var note in notes)
        {
            targetNotesLayer.Add(note.Visual);
        }
    }

    public void UpdateBeat(double currentBeat) => this.currentBeat = currentBeat;

    /// <summary>
    /// Adds a new note to display. Even tough this code in theory be any note, for logical reasons it should only be a note, whoms UsdxNoteType == Sung.
    /// Other types of notes can also be added but will be represented based on their type.
    /// </summary>
    /// <param name="sungNote"></param>
    public void AddSungNote(INote sungNote) => sungNotesLayer.Add(sungNote.Visual);

    protected override void Update()
    {
        base.Update();

        if (naturalPhraseWidth <= 0 || DrawWidth <= 0) return;

        // Calculate dynamic scale factor to make the phrase fit the available screen width
        float stretchScale = Math.Min(DrawWidth / naturalPhraseWidth, 3);

        // Stretch the container holding all the visual notes
        targetNotesLayer.Scale = new Vector2(stretchScale, 1f);

        // Shift the layer left, adjusting for scale, so the first note sits exactly at X = 0
        targetNotesLayer.X = -phraseStartOffset * stretchScale;
        sungNotesLayer.X = targetNotesLayer.X;

        // Scale the playhead position simultaneously
        playhead.X = ((float)(currentBeat * UsdxNote.SCALE_FACTOR) - phraseStartOffset) * stretchScale;
    }
}
