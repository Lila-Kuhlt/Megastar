using osu.Framework.Graphics;

namespace megastar.Game.notes;

public interface INote
{
    uint StartBeat { get; set; }
    public int Length { get; set; }
    public int Pitch { get; set; }
    public string Text { get; set; }
    public UsdxNoteType NoteType { get; set; }
    Drawable Visual { get; }
}
