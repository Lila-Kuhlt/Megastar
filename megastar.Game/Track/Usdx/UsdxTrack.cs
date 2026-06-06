using System;
using System.Collections.Generic;
using System.IO;
using megastar.Game.notes;

namespace megastar.Game.Track.Usdx;

public class UsdxTrack : ITrack
{
    public string Artist { get; private set; }
    public string Title { get; private set; }
    public string Creator { get; private set; }
    public int Length { get; private set; }
    public double Bpm { get; private set; }
    public string Version { get; private set; }
    public string AudioFile { get; private set; }
    public string MetadataFile { get; private set; }
    public string DirPath { get; private set; }
    public string? BackgroundImageFile { get; private set; }
    public string? BackgroundVideoFile { get; private set; }
    public double VideoGap { get; private set; }
    public double Gap { get; private set; }

    public Lazy<List<IBeatPaced>> Notes { get; set; }

    private UsdxTrack() => Notes = new Lazy<List<IBeatPaced>>(loadNotes);

    public UsdxTrack(ITrackMetadata metadata) : this()
    {
        Artist = metadata.Artist;
        Title = metadata.Title;
        Creator = metadata.Creator;
        Length = metadata.Length;
        Bpm = metadata.Bpm;
        Version = metadata.Version;
        AudioFile = metadata.AudioFile;
        MetadataFile = metadata.MetadataFile;
        DirPath = metadata.DirPath;
        BackgroundImageFile = metadata.BackgroundImageFile;
        BackgroundVideoFile = metadata.BackgroundVideoFile;
        VideoGap = metadata.VideoGap;
        Gap = metadata.Gap;
    }

    private List<IBeatPaced> loadNotes()
    {
        if (string.IsNullOrEmpty(MetadataFile) || string.IsNullOrEmpty(MetadataFile) ||
            !File.Exists(MetadataFile)) return [];
        return UsdxParser.ParseUsdxNotes(File.ReadAllText(MetadataFile));
    }
}
