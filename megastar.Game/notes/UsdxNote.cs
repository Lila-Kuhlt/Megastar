using JetBrains.Annotations;

namespace megastar.Game.notes;

public class UsdxNote(uint startBeat, int lenght, int pitch, [CanBeNull] string text, UsdxNoteType noteType)
    : INote
{
    public uint startBeat { get; set; } = startBeat;
    public int lenght { get; set; } = lenght;
    public int pitch { get; set; } = pitch;
    public string text { get; set; } = text;
    public UsdxNoteType noteType { get; set; } = noteType;
}
