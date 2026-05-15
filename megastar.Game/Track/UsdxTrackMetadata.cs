using System;
using System.Collections.Generic;

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

    public UsdxTrackMetadata(string artist, string title, string creator, uint length, double bpm, string version, string songFile, string backgroundImageFile)
        : this()
    {
        this.Artist = artist;
        this.Title = title;
        this.Creator = creator;
        this.Length = length;
        this.BPM = bpm;
        this.Version = version;
        this.SongFile = songFile;
        this.BackgroundImageFile = backgroundImageFile;
    }

    public UsdxTrackMetadata(Dictionary<string, string> trackMetadata)
        : this()
    {
        if (trackMetadata.ContainsKey("artist"))
            Artist = (string)trackMetadata["artist"];
        if (trackMetadata.ContainsKey("title"))
            Title = (string)trackMetadata["title"];
        if (trackMetadata.ContainsKey("creator"))
            Creator = (string)trackMetadata["creator"];
        if (trackMetadata.ContainsKey("length"))
            Length = Convert.ToUInt32(trackMetadata["length"]);
        if (trackMetadata.ContainsKey("bpm"))
            BPM = Convert.ToUInt32(trackMetadata["bpm"]);
        if (trackMetadata.ContainsKey("version"))
            Version = (string)trackMetadata["version"];
        if (trackMetadata.ContainsKey("songFile"))
            SongFile = (string)trackMetadata["songFile"];
        if (trackMetadata.ContainsKey("backgroundImageFile"))
            BackgroundImageFile = (string)trackMetadata["backgroundImageFile"];
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
