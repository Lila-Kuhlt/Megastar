using System;
using System.Collections.Generic;
using System.IO;
using megastar.Game.notes;

namespace megastar.Game.Track;

public class LazyLoadedTrackData(ITrackMetadata metadata) : ITrackData
{
    public Lazy<List<IBeatPaced>> Notes { get; } = new(() => LoadNotes(metadata));

    private static List<IBeatPaced> LoadNotes(ITrackMetadata metadata)
    {
        var fileName = metadata.MetadataFile;
        var filePath = metadata.MetadataFilePath();

        if (string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(metadata.DirPath) ||
            !File.Exists(filePath)) return [];

        return UsdxParser.ParseUsdxNotes(File.ReadAllText(filePath));
    }
}
