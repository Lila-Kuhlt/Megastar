namespace megastar.Game.notes;

public class UsdxPauseNote(uint startBeat) : IBeatPaced
{
    public uint StartBeat { get; set; } = startBeat;
}
