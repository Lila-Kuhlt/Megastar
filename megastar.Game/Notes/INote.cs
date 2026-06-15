using osu.Framework.Graphics;

namespace megastar.Game.notes;

public interface INote : IBeatPaced
{
    new int StartBeat { get; set; }
    new int Length { get; set; }
    int Pitch { get; set; }
    string Text { get; set; }
    UsdxNoteType NoteType { get; set; }
    new Drawable Visual { get; }
}
