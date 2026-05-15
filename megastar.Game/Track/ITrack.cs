using System.Collections.Generic;
using megastar.Game.notes;

namespace megastar.Game.Track;

public interface ITrack
{
    ITrackMetadata TrackMetadata { get; set; }
    List<INote> Notes { get; set; }
}
