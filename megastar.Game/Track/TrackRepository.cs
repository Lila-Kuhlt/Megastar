using System;
using System.Collections.Generic;
using System.Linq;
using megastar.Game.Track.Megastar;
using osu.Framework.Extensions.IEnumerableExtensions;
using Realms;

namespace megastar.Game.Track;

public class TrackRepository() : RealmRepository("megastar.realm")
{
    /// <summary>
    /// Returns _EVERY_ track stored in the repository. THIS IS VERY EXPENSIVE.
    /// Use <code>Run(realm => realm.All() ... )</code> if you only need a subset of every track.
    /// </summary>
    /// <returns>Every track stored in the Realm db</returns>
    public IEnumerable<MegastarTrackMetadata> AllTracks() => Run(realm => realm.All<MegastarTrackMetadata>().ToList());

    public void Add(MegastarTrackMetadata track) => Write(realm => realm.Add(track, true));

    /// <summary>
    /// Inserts provided tracks into the DB as batch operation. Locks the DB for the time the operation is used.
    /// Most of the time <see cref="TrackRepository.Add"/> in a loop is sufficient (and supports live updates!).
    /// </summary>
    /// <param name="tracks"></param>
    public void AddMultiple(IEnumerable<MegastarTrackMetadata> tracks) =>
        Write(realm =>
        {
            foreach (var track in tracks) realm.Add(track, true);
        });
}
