using System;
using JetBrains.Annotations;

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
    /// Song path, relative to the song directory
    /// </summary>
    string SongFile { get; set; }

    /// <summary>
    /// The Path of the text file
    /// </summary>
    string Path { get; set; }

    /// <summary>
    /// The Path of the directory
    /// </summary>
    string DirPath { get; set; }

    /// <summary>
    /// Background image path, relative to the song directory
    /// </summary>
    [CanBeNull]
    string BackgroundImageFile { get; set; }

    /// <summary>
    /// Background video path, relative to the song directory
    /// </summary>
    [CanBeNull]
    string BackgroundVideoFile { get; set; }

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
    public static void CollectMetadataFrom(this ITrackMetadata curr, [CanBeNull] ITrackMetadata other)
    {
        if (other == null) return;

        curr.Artist = other.Artist;
        curr.Title = other.Title;
        curr.Creator = other.Creator;
        curr.Length = other.Length;
        curr.Bpm = other.Bpm;
        curr.Version = other.Version;
        curr.SongFile = other.SongFile;
        curr.Path = other.Path;
        curr.DirPath = other.DirPath;
        curr.BackgroundImageFile = other.BackgroundImageFile;
        curr.BackgroundVideoFile = other.BackgroundVideoFile;
        curr.VideoGap = other.VideoGap;
        curr.Gap = other.Gap;
    }
}
