using System;
using System.Collections.Generic;
using System.IO;
using megastar.Game.notes;
using megastar.Game.Track.Usdx;

namespace megastar.Game.Track;

public class LazyLoadedTrackData(ITrackMetadata metadata) : ITrackData
{
    private Lazy<List<IBeatPaced>> notes { get; } = new(() => loadNotes(metadata));
    public List<IBeatPaced> Notes => notes.Value;

    private static List<IBeatPaced> loadNotes(ITrackMetadata metadata)
    {
        var fileName = metadata.MetadataFile;
        var filePath = metadata.MetadataFilePath();

        if (string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(metadata.DirPath) ||
            !File.Exists(filePath)) return [];

        return UsdxParser.ParseUsdxNotes(File.ReadAllText(filePath));
    }
}
