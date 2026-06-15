using osu.Framework.Graphics;

namespace megastar.Game.notes;

public interface IBeatPaced
{
    int StartBeat { get; }
    int Length { get; set; }
    Drawable Visual { get; }
}
