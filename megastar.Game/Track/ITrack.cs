
using osu.Framework.Graphics.Containers;

namespace megastar.Game;

public interface ITrack : IFilterable
{
    ITrackMetadata trackMetadata { get; set; }
    IFilterable GetVisualRepresentation { get; }
}
