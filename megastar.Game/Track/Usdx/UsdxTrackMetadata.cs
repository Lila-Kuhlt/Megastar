namespace megastar.Game.Track.Usdx;

public record UsdxTrackMetadata(
    string Artist,
    string Title,
    string Creator,
    int Length,
    double Bpm,
    string Version,
    string AudioFile,
    string? BackgroundImageFile,
    string? BackgroundVideoFile,
    string MetadataFile,
    double Gap,
    double VideoGap,
    string DirPath)
    : ITrackMetadata
{
    public string Artist { get; set; } = Artist;
    public string Title { get; set; } = Title;
    public string Creator { get; set; } = Creator;
    public int Length { get; set; } = Length;
    public double Bpm { get; set; } = Bpm;
    public string Version { get; set; } = Version;
    public string AudioFile { get; set; } = AudioFile;
    public string? BackgroundImageFile { get; set; } = BackgroundImageFile;
    public string? BackgroundVideoFile { get; set; } = BackgroundVideoFile;

    public string MetadataFile { get; set; } = MetadataFile;
    public double Gap { get; set; } = Gap;
    public double VideoGap { get; set; } = VideoGap;
    public string DirPath { get; set; } = DirPath;


    public override string ToString()
        => $"{Artist} - {Title}";
}
