using System.IO;

namespace megastar.Game.Track;

public interface ITrackMetadata
{
    /// <summary>
    /// Song artist(s)
    /// </summary>
    string Artist { get; set; }

    /// <summary>
    /// Song title
    /// </summary>
    string Title { get; set; }

    /// <summary>
    ///  Creator of the track file (usdb txt file)
    /// </summary>
    string Creator { get; set; }

    /// <summary>
    ///  Length of the songs
    /// </summary>
    int Length { get; set; }

    /// <summary>
    /// Song BPM
    /// </summary>
    double Bpm { get; set; }

    /// <summary>
    /// USDX File version
    /// </summary>
    string Version { get; set; }

    /// <summary>
    /// The Path of the directory
    /// </summary>
    string DirPath { get; set; }

    /// <summary>
    /// Song path, relative to the song directory
    /// </summary>
    string AudioFile { get; set; }

    /// <summary>
    /// The Path of the text file containing all metadata
    /// </summary>
    string MetadataFile { get; set; }

    /// <summary>
    /// Background image path, relative to the song directory
    /// </summary>
    string? BackgroundImageFile { get; set; }

    /// <summary>
    /// Background video path, relative to the song directory
    /// </summary>
    string? BackgroundVideoFile { get; set; }

    /// <summary>
    /// Background video path, relative to the song directory
    /// </summary>
    double VideoGap { get; set; }

    /// <summary>
    /// Gap between the start of the song and the first note. Should be 0 if none exists
    /// </summary>
    double Gap { get; set; }
}

public static class TrackMetadataExtensions
{
    /// <summary>
    /// Helper function to apply ITrackMetadata to the current metadata.
    /// This can be helpful when transferring file types. e.g. from USDX to Megastar
    /// </summary>
    public static void CollectMetadataFrom(this ITrackMetadata curr, ITrackMetadata? other)
    {
        if (other == null) return;

        curr.Artist = other.Artist;
        curr.Title = other.Title;
        curr.Creator = other.Creator;
        curr.Length = other.Length;
        curr.Bpm = other.Bpm;
        curr.Version = other.Version;
        curr.AudioFile = other.AudioFile;
        curr.MetadataFile = other.MetadataFile;
        curr.DirPath = other.DirPath;
        curr.BackgroundImageFile = other.BackgroundImageFile;
        curr.BackgroundVideoFile = other.BackgroundVideoFile;
        curr.VideoGap = other.VideoGap;
        curr.Gap = other.Gap;
    }

    public static string AudioFilePath(this ITrackMetadata metadata) =>
        Path.Combine(metadata.DirPath, metadata.AudioFile);

    public static string MetadataFilePath(this ITrackMetadata metadata) =>
        Path.Combine(metadata.DirPath, metadata.MetadataFile);


    public static string? BackgroundImageFilePath(this ITrackMetadata metadata) =>
        metadata.BackgroundImageFile != null ? Path.Combine(metadata.DirPath, metadata.BackgroundImageFile) : null;

    public static string? BackgroundVideoFilePath(this ITrackMetadata metadata) =>
        metadata.BackgroundVideoFile != null ? Path.Combine(metadata.DirPath, metadata.BackgroundVideoFile) : null;
}
