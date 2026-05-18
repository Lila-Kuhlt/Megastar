namespace megastar.Game.notes;

public interface INote : IBeatPaced
{
    uint StartBeat { get; }

    int Length { get; }
    int Pitch { get; }
    string Text { get; }
    UsdxNoteType NoteType { get; }
}
