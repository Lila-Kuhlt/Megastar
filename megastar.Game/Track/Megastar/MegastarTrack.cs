using System;
using System.Collections.Generic;
using megastar.Game.notes;

namespace megastar.Game.Track.Megastar;

public class MegastarTrack : ITrack, IVerifiableMetadata
{
    private readonly IVerifiableMetadata trackMetadata;
    public Lazy<List<IBeatPaced>> Notes { get; set; }

    public MegastarTrack(IVerifiableMetadata trackMetadata)
    {
        this.trackMetadata = trackMetadata;
        Notes = new LazyLoadedTrackData(this).Notes;
    }

    public string Artist
    {
        get => trackMetadata.Artist;
        set => trackMetadata.Artist = value;
    }

    public string Title
    {
        get => trackMetadata.Title;
        set => trackMetadata.Title = value;
    }

    public string Creator
    {
        get => trackMetadata.Creator;
        set => trackMetadata.Creator = value;
    }

    public int Length
    {
        get => trackMetadata.Length;
        set => trackMetadata.Length = value;
    }

    public double Bpm
    {
        get => trackMetadata.Bpm;
        set => trackMetadata.Bpm = value;
    }

    public string Version
    {
        get => trackMetadata.Version;
        set => trackMetadata.Version = value;
    }

    public string DirPath
    {
        get => trackMetadata.DirPath;
        set => trackMetadata.DirPath = value;
    }

    public string AudioFile
    {
        get => trackMetadata.AudioFile;
        set => trackMetadata.AudioFile = value;
    }

    public string MetadataFile
    {
        get => trackMetadata.MetadataFile;
        set => trackMetadata.MetadataFile = value;
    }

    public string? BackgroundImageFile
    {
        get => trackMetadata.BackgroundImageFile;
        set => trackMetadata.BackgroundImageFile = value;
    }

    public string? BackgroundVideoFile
    {
        get => trackMetadata.BackgroundVideoFile;
        set => trackMetadata.BackgroundVideoFile = value;
    }

    public double VideoGap
    {
        get => trackMetadata.VideoGap;
        set => trackMetadata.VideoGap = value;
    }

    public double Gap
    {
        get => trackMetadata.Gap;
        set => trackMetadata.Gap = value;
    }

    public byte[]? AudioFileHash
    {
        get => trackMetadata.AudioFileHash;
        set => trackMetadata.AudioFileHash = value;
    }

    public byte[]? MetadataFileHash
    {
        get => trackMetadata.MetadataFileHash;
        set => trackMetadata.MetadataFileHash = value;
    }

    public byte[]? BackgroundImageHash
    {
        get => trackMetadata.BackgroundImageHash;
        set => trackMetadata.BackgroundImageHash = value;
    }

    public byte[]? BackgroundVideoHash
    {
        get => trackMetadata.BackgroundVideoHash;
        set => trackMetadata.BackgroundVideoHash = value;
    }

    public DateTimeOffset LastVerified
    {
        get => trackMetadata.LastVerified;
        set => trackMetadata.LastVerified = value;
    }
}
