using System;
using System.Collections.Generic;
using System.Linq;
using megastar.Game.Track.Megastar;
using osu.Framework.Extensions.IEnumerableExtensions;
using Realms;

namespace megastar.Game.Track;

public class TrackRepository() : RealmRepository("megastar.realm")
{
    public IEnumerable<MegastarTrackMetadata> AllTracks() => Run(realm => realm.All<MegastarTrackMetadata>().ToList());

    public void Add(MegastarTrackMetadata track) => Write(realm => realm.Add(track, true));

    public void AddMultiple(IEnumerable<MegastarTrackMetadata> tracks) =>
        Write(realm =>
        {
            foreach (var track in tracks) realm.Add(track, true);
        });
}
