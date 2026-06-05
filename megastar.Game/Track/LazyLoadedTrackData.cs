using System;
using System.Collections.Generic;
using System.IO;
using megastar.Game.notes;

namespace megastar.Game.Track;

public record LazyLoadedTrackData : ITrackData
{
    public Lazy<List<IBeatPaced>> Notes { get; set; }

    public LazyLoadedTrackData(ITrackMetadata metadata) =>
        Notes = new Lazy<List<IBeatPaced>>(() => loadNotes(metadata));

    private static List<IBeatPaced> loadNotes(ITrackMetadata metadata)
    {
        if (string.IsNullOrEmpty(metadata.MetadataFile) || string.IsNullOrEmpty(metadata.MetadataFile) ||
            !File.Exists(metadata.MetadataFile)) return [];
        return UsdxParser.ParseUsdxNotes(File.ReadAllText(metadata.MetadataFile));
    }
}
