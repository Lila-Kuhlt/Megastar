using System.Collections.Generic;
using System.IO;
using megastar.Game.notes;

namespace megastar.Game.Track;

public interface ITrackMetadata
{
    /// <summary>
    /// Song artist(s)
    /// </summary>
    string Artist { get; }

    /// <summary>
    /// Song title
    /// </summary>
    string Title { get; }

    /// <summary>
    ///  Creator of the track file (usdb txt file)
    /// </summary>
    string Creator { get; }

    /// <summary>
    ///  Length of the songs
    /// </summary>
    int Length { get; }

    /// <summary>
    /// Song BPM
    /// </summary>
    double Bpm { get; }

    /// <summary>
    /// USDX File version
    /// </summary>
    string Version { get; }

    /// <summary>
    /// The Path of the directory
    /// </summary>
    string DirPath { get; }

    /// <summary>
    /// Song path, relative to the song directory
    /// </summary>
    string AudioFile { get; }

    /// <summary>
    /// The Path of the text file containing all metadata
    /// </summary>
    string MetadataFile { get; }

    /// <summary>
    /// Background image path, relative to the song directory
    /// </summary>
    string? BackgroundImageFile { get; }

    /// <summary>
    /// Background video path, relative to the song directory
    /// </summary>
    string? BackgroundVideoFile { get; }

    /// <summary>
    /// Background video path, relative to the song directory
    /// </summary>
    double VideoGap { get; }

    /// <summary>
    /// Gap between the start of the song and the first note. Should be 0 if none exists
    /// </summary>
    double Gap { get; }
}

public static class TrackMetadataExtensions
{
    public static string AudioFilePath(this ITrackMetadata metadata) =>
        Path.Combine(metadata.DirPath, metadata.AudioFile);

    public static string MetadataFilePath(this ITrackMetadata metadata) =>
        Path.Combine(metadata.DirPath, metadata.MetadataFile);


    public static string? BackgroundImageFilePath(this ITrackMetadata metadata) =>
        metadata.BackgroundImageFile != null ? Path.Combine(metadata.DirPath, metadata.BackgroundImageFile) : null;

    public static string? BackgroundVideoFilePath(this ITrackMetadata metadata) =>
        metadata.BackgroundVideoFile != null ? Path.Combine(metadata.DirPath, metadata.BackgroundVideoFile) : null;


    public static List<IBeatPaced> LoadNotes(this ITrackMetadata metadata)
    {
        if (string.IsNullOrEmpty(metadata.DirPath) || string.IsNullOrEmpty(metadata.MetadataFile) ||
            !File.Exists(metadata.MetadataFilePath()))
            return [];

        return UsdxParser.ParseUsdxNotes(File.ReadAllText(metadata.MetadataFilePath()));
    }
}
