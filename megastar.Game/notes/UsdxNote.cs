using JetBrains.Annotations;

namespace megastar.Game.notes;

public class UsdxNote(uint startBeat, uint lenght, uint pitch, [CanBeNull] string text, UsdxNoteType noteType)
    : INote
{
    public uint startBeat { get; set; } = startBeat;
    public uint lenght { get; set; } = lenght;
    public uint pitch { get; set; } = pitch;
    public string text { get; set; } = text;
    public UsdxNoteType noteType { get; set; } = noteType;
}
