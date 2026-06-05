using System;
using System.Collections.Generic;
using System.Globalization;

namespace megastar.Game.Track.Usdx;

public record UsdxTrackMetadata(
    string artist,
    string title,
    string creator,
    int length,
    double bpm,
    string version,
    string audioFile,
    string? backgroundImageFile,
    string? backgroundVideoFile,
    string metadataFile,
    double gap,
    double videoGap,
    string dirPath)
    : ITrackMetadata
{
    public string Artist { get; set; } = artist;
    public string Title { get; set; } = title;
    public string Creator { get; set; } = creator;
    public int Length { get; set; } = length;
    public double Bpm { get; set; } = bpm;
    public string Version { get; set; } = version;
    public string AudioFile { get; set; } = audioFile;
    public string? BackgroundImageFile { get; set; } = backgroundImageFile;
    public string? BackgroundVideoFile { get; set; } = backgroundVideoFile;

    public string MetadataFile { get; set; } = metadataFile;
    public double Gap { get; set; } = gap;
    public double VideoGap { get; set; } = videoGap;
    public string DirPath { get; set; } = dirPath;


    public override string ToString()
        => $"{Artist} - {Title}";
}
