using System.Collections.Generic;
using megastar.Game.notes;

namespace megastar.Game;

public class UsdxTrack(ITrackMetadata trackMetadata, List<INote> notes) : ITrack
{
    // Auto-property handles both getting and setting automatically
    public ITrackMetadata TrackMetadata { get; set; } = trackMetadata;
    public List<INote> Notes { get; set; } = notes;


}
