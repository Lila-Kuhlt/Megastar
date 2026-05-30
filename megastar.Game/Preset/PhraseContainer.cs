using System.Collections.Generic;
using System.Linq;
using megastar.Game.notes;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

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

    private readonly float offsetX;
    private readonly float offsetY;

    /// <summary>
    /// This will instantiate a new container with the given Notes. It will only use the pitch and length to visualize
    /// </summary>
    /// <param name="notes">The Notes to display</param>
    public NoteContainer(List<INote> notes)
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
        AddInternal(sungNotesLayer);
        AddInternal(playhead);

        if (notes.Count <= 0) return;

        // Pins the first note 210px from the left edge
        int phraseStartBeat = notes[0].StartBeat;
        offsetX = -phraseStartBeat * UsdxNote.SCALE_FACTOR + 210f;

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

        targetNotesLayer.X = offsetX;
        targetNotesLayer.Y = offsetY;

        sungNotesLayer.X = offsetX;
        sungNotesLayer.Y = offsetY;

        foreach (var note in notes)
        {
            targetNotesLayer.Add(note.Visual);
        }
    }

    public void UpdateBeat(double currentBeat) => playhead.X = (float)(currentBeat * UsdxNote.SCALE_FACTOR) + offsetX;

    /// <summary>
    /// Adds a new note to display. Even tough this code in theory be any note, for logical reasons it should only be a note, whoms UsdxNoteType == Sung.
    /// Other types of notes can also be added but will be represented based on their type.
    /// </summary>
    /// <param name="sungNote"></param>
    public void AddSungNote(INote sungNote) => sungNotesLayer.Add(sungNote.Visual);
}
