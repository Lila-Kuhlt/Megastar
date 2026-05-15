namespace megastar.Game.notes;

public class UsdxPauseNote(uint startBeat) : INote
{
    public uint StartBeat { get; set; } = startBeat;
}
