namespace megastar.Game.notes;

public class UsdxPauseNote(uint startBeat) : INote
{
    public uint startBeat { get; set; } = startBeat;
}
