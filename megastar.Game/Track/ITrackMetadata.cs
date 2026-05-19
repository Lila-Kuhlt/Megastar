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
    uint Length { get; set; }

    /// <summary>
    /// Song BPM
    /// </summary>
    double BPM { get; set; }

    /// <summary>
    /// USDX File version
    /// </summary>
    string Version { get; set; }

    /// <summary>
    /// Song path, relative to the song directory
    /// </summary>
    string SongFile { get; set; }

    [CanBeNull] string Path { get; set; }

    /// <summary>
    /// Background image path, relative to the song directory
    /// </summary>
    [CanBeNull]
    string BackgroundImageFile { get; set; }

    /// <summary>
    /// Gap between the start of the song and the first note. Should be 0 if none exists
    /// </summary>
    double Gap { get; set; }
}
