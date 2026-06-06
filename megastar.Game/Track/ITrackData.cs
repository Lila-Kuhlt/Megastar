using System;
using System.Collections.Generic;
using megastar.Game.notes;

namespace megastar.Game.Track;

public interface ITrackData
{
    Lazy<List<IBeatPaced>> Notes { get; }
}
