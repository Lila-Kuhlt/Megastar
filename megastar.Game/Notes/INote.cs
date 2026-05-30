using osu.Framework.Graphics;

namespace megastar.Game.notes;

public interface INote : IBeatPaced
{
    int StartBeat { get; set; }
    int Length { get; set; }
    int Pitch { get; set; }
    string Text { get; set; }
    UsdxNoteType NoteType { get; set; }
    Drawable Visual { get; }
}
