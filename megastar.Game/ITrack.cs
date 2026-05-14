namespace megastar.Game;

public interface ITrack
{
    ITrackMetadata trackMetadata { get; set; }
}

public interface ITrackMetadata
{
    /// <summary>
    /// Song artist(s)
    /// </summary>
    string artist { get; set; }

    /// <summary>
    /// Song title
    /// </summary>
    string title { get; set; }

    /// <summary>
    ///  Creator of the track file (usdb txt file)
    /// </summary>
    string creator { get; set; }

    /// <summary>
    ///  Length of the songs
    /// </summary>
    uint length { get; set; }

    /// <summary>
    /// Song BPM
    /// </summary>
    uint bpm { get; set; }

    /// <summary>
    /// USDX File version
    /// </summary>
    string version { get; set; }

    /// <summary>
    /// Song path, relative to the song directory
    /// </summary>
    string songFile { get; set; }

    /// <summary>
    /// Background image path, relative to the song directory
    /// </summary>
    string backgroundImageFile { get; set; }
}
