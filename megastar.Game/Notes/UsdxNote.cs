using JetBrains.Annotations;

namespace megastar.Game.notes;

public class UsdxNote(uint startBeat, int lenght, int pitch, [CanBeNull] string text, UsdxNoteType noteType)
    : INote
{
    public uint StartBeat { get; set; } = startBeat;
    public int Lenght { get; set; } = lenght;
    public int Pitch { get; set; } = pitch;
    public string Text { get; set; } = text;
    public UsdxNoteType NoteType { get; set; } = noteType;
}
