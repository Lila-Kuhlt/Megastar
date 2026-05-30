using System;
using System.Collections.Generic;
using System.IO;
using megastar.Game.notes;

namespace megastar.Game.Track;

public class UsdxTrack : ITrack
{
    public string Artist { get; set; }
    public string Title { get; set; }
    public string Creator { get; set; }
    public int Length { get; set; }
    public double Bpm { get; set; }
    public string Version { get; set; }
    public string SongFile { get; set; }
    public string Path { get; set; }
    public string DirPath { get; set; }
    public string BackgroundImageFile { get; set; }
    public string BackgroundVideoFile { get; set; }
    public double VideoGap { get; set; }
    public double Gap { get; set; }

    public Lazy<List<IBeatPaced>> Notes { get; set; }

    private UsdxTrack() => Notes = new Lazy<List<IBeatPaced>>(loadNotes);

    public UsdxTrack(ITrackMetadata metadata) : this() => this.CollectMetadataFrom(metadata);

    private List<IBeatPaced> loadNotes()
    {
        if (string.IsNullOrEmpty(Path) || string.IsNullOrEmpty(Path) || !File.Exists(Path)) return [];
        return Parser.ParseUsdxNotes(File.ReadAllText(Path));
    }
}
