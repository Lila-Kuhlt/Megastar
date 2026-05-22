using osu.Framework.Graphics;

namespace megastar.Game.notes;

public interface IBeatPaced
{
    uint StartBeat { get; }
    Drawable Visual { get; }
}
