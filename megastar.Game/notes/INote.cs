namespace megastar.Game.notes;

public interface INote
{
    uint startBeat {get; set; }
    uint lenght { get; set; }
    uint pitch { get; set; }
    string text {get; set; }
}
