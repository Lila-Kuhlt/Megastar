using JetBrains.Annotations;

namespace megastar.Game.notes;

public class UsdxNote(uint startBeat, int length, int pitch, [CanBeNull] string text, UsdxNoteType noteType)
    : INote
{
    public uint StartBeat { get; } = startBeat;
    public int Length { get; } = length;
    public int Pitch { get; } = pitch;
    public string Text { get; } = text;
    public UsdxNoteType NoteType { get; } = noteType;
}
