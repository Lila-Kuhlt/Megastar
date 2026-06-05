using System;
using System.Collections.Generic;
using System.IO;
using megastar.Game.notes;

namespace megastar.Game.Track.Usdx;

public class UsdxTrack : ITrack
{
    public string Artist { get; set; }
    public string Title { get; set; }
    public string Creator { get; set; }
    public int Length { get; set; }
    public double Bpm { get; set; }
    public string Version { get; set; }
    public string AudioFile { get; set; }
    public string MetadataFile { get; set; }
    public string DirPath { get; set; }
    public string? BackgroundImageFile { get; set; }
    public string? BackgroundVideoFile { get; set; }
    public double VideoGap { get; set; }
    public double Gap { get; set; }

    public Lazy<List<IBeatPaced>> Notes { get; set; }

    private UsdxTrack() => Notes = new Lazy<List<IBeatPaced>>(loadNotes);

    public UsdxTrack(ITrackMetadata metadata) : this() => this.CollectMetadataFrom(metadata);

    private List<IBeatPaced> loadNotes()
    {
        if (string.IsNullOrEmpty(MetadataFile) || string.IsNullOrEmpty(MetadataFile) ||
            !File.Exists(MetadataFile)) return [];
        return UsdxParser.ParseUsdxNotes(File.ReadAllText(MetadataFile));
    }
}
