using osu.Framework.Graphics;

namespace megastar.Game.notes;

public interface IBeatPaced
{
    uint StartBeat { get; }
    int Length { get; set; }
    Drawable Visual { get; }
}
