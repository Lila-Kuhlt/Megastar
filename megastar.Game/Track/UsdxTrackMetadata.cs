using JetBrains.Annotations;

namespace megastar.Game;

public class UsdxTrackMetadata(string artist, string title, string creator, uint length, double bpm, string version, string songFile, [CanBeNull] string backgroundImageFile)
    : ITrackMetadata
{
    public string artist { get; set; } = artist;
    public string title { get; set; } = title;
    public string creator { get; set; } = creator;
    public uint length { get; set; } = length;
    public double bpm { get; set; } = bpm;
    public string version { get; set; } = version;
    public string songFile { get; set; } = songFile;
    public string backgroundImageFile { get; set; } = backgroundImageFile;
}
