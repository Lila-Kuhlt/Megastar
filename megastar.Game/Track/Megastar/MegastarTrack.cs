using System;
using System.Collections.Generic;
using megastar.Game.notes;

namespace megastar.Game.Track.Megastar;

public partial class MegastarTrack(IVerifiableMetadata trackMetadata) : ITrack, IVerifiableMetadata
{
    public Lazy<List<IBeatPaced>> Notes { get; } = new(trackMetadata.LoadNotes);

    public string Artist => trackMetadata.Artist;

    public string Title => trackMetadata.Title;

    public string Creator => trackMetadata.Creator;

    public int Length => trackMetadata.Length;

    public double Bpm => trackMetadata.Bpm;

    public string Version => trackMetadata.Version;

    public string DirPath => trackMetadata.DirPath;

    public string AudioFile => trackMetadata.AudioFile;

    public string MetadataFile => trackMetadata.MetadataFile;

    public string? BackgroundImageFile => trackMetadata.BackgroundImageFile;

    public string? BackgroundVideoFile => trackMetadata.BackgroundVideoFile;

    public double VideoGap => trackMetadata.VideoGap;

    public double Gap => trackMetadata.Gap;

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
