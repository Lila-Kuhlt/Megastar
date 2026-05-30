using System;
using System.Collections.Generic;
using System.Globalization;

namespace megastar.Game.Track;

public class UsdxTrackMetadata()
    : ITrackMetadata
{
    public string Artist { get; set; }
    public string Title { get; set; }
    public string Creator { get; set; }
    public int Length { get; set; }
    public double Bpm { get; set; }
    public string Version { get; set; }
    public string SongFile { get; set; }
    public string BackgroundImageFile { get; set; }
    public string BackgroundVideoFile { get; set; }

    public string Path { get; set; }
    public double Gap { get; set; }
    public double VideoGap { get; set; }
    public string DirPath { get; set; }

    public UsdxTrackMetadata(Dictionary<string, string> trackMetadata)
        : this()
    {
        if (trackMetadata.TryGetValue("artist", out var artist))
            Artist = artist;
        if (trackMetadata.TryGetValue("title", out var title))
            Title = title;
        if (trackMetadata.TryGetValue("creator", out var creator))
            Creator = creator;
        if (trackMetadata.TryGetValue("length", out var length))
            Length = Convert.ToInt32(length);
        if (trackMetadata.TryGetValue("bpm", out var bpm))
            Bpm = Convert.ToDouble(bpm, CultureInfo.InvariantCulture);
        if (trackMetadata.TryGetValue("version", out var version))
            Version = version;
        if (trackMetadata.TryGetValue("songFile", out var songFile))
            SongFile = songFile;
        if (trackMetadata.TryGetValue("mp3", out var mp3))
            SongFile = mp3;
        if (trackMetadata.TryGetValue("audio", out var audio))
            SongFile = audio;
        if (trackMetadata.TryGetValue("video", out var video))
            BackgroundVideoFile = video;
        if (trackMetadata.TryGetValue("background", out var bg))
            BackgroundImageFile = bg;
        if (trackMetadata.TryGetValue("gap", out var gap))
            Gap = Convert.ToDouble(gap, CultureInfo.InvariantCulture);
    }

    public override string ToString()
        => $"{Artist} - {Title}";
}
