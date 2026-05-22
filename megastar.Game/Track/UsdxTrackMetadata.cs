using System;
using System.Collections.Generic;
using System.Globalization;
using megastar.Game.Track;

namespace megastar.Game;

public class UsdxTrackMetadata()
    : ITrackMetadata
{
    public string Artist { get; set; }
    public string Title { get; set; }
    public string Creator { get; set; }
    public uint Length { get; set; }
    public double BPM { get; set; }
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
        if (trackMetadata.ContainsKey("artist"))
            Artist = trackMetadata["artist"];
        if (trackMetadata.ContainsKey("title"))
            Title = trackMetadata["title"];
        if (trackMetadata.ContainsKey("creator"))
            Creator = trackMetadata["creator"];
        if (trackMetadata.ContainsKey("length"))
            Length = Convert.ToUInt32(trackMetadata["length"]);
        if (trackMetadata.ContainsKey("bpm"))
            BPM = Convert.ToDouble(trackMetadata["bpm"], CultureInfo.InvariantCulture);
        if (trackMetadata.ContainsKey("version"))
            Version = trackMetadata["version"];
        if (trackMetadata.ContainsKey("songFile"))
            SongFile = trackMetadata["songFile"];
        if (trackMetadata.ContainsKey("mp3"))
            SongFile = trackMetadata["mp3"];
        if (trackMetadata.ContainsKey("audio"))
            SongFile = trackMetadata["audio"];
        if (trackMetadata.ContainsKey("video"))
            BackgroundVideoFile = trackMetadata["video"];
        if (trackMetadata.ContainsKey("background"))
            BackgroundImageFile = trackMetadata["background"];
        if (trackMetadata.ContainsKey("gap"))
            Gap = Convert.ToDouble(trackMetadata["gap"], CultureInfo.InvariantCulture);
    }

    public new void ToString()
    {
        Console.WriteLine("Artist: " + Artist);
        Console.WriteLine("Title: " + Title);
        Console.WriteLine("Creator: " + Creator);
        Console.WriteLine("Length: " + Length);
        Console.WriteLine("BPM: " + BPM);
        Console.WriteLine("Version: " + Version);
        Console.WriteLine("Song File: " + SongFile);
        Console.WriteLine("Background Image: " + BackgroundImageFile);
    }
}
