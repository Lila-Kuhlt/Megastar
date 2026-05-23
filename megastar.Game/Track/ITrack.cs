using System.Collections.Generic;
using megastar.Game.notes;

namespace megastar.Game.Track;

public interface ITrack
{
    ITrackMetadata TrackMetadata { get; set; }
    List<IBeatPaced> Notes { get; set; }
    List<List<IBeatPaced>> NotePhrases { get; }

    public void clearStorage();
}
