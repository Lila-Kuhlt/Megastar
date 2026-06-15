using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using MongoDB.Bson;
using Realms;

namespace megastar.Game.Track.Megastar;

public partial class MegastarTrackMetadata : IVerifiableMetadata, IRealmObject
{
    [MapTo("_id")]
    [PrimaryKey]
    public ObjectId Id { get; private set; } = ObjectId.GenerateNewId(); // TODO: Get Id from USDX, OR generate new

    public byte[]? AudioFileHash { get; set; }
    public byte[]? MetadataFileHash { get; set; }
    public byte[]? BackgroundImageHash { get; set; }
    public byte[]? BackgroundVideoHash { get; set; }
    public DateTimeOffset LastVerified { get; set; }

    private int sampleStart;
    private int sampleLength;

    public MegastarTrackMetadata(ITrackMetadata metadata)
    {
        Artist = metadata.Artist;
        Title = metadata.Title;
        Creator = metadata.Creator;
        Length = metadata.Length;
        Bpm = metadata.Bpm;
        Version = metadata.Version;
        AudioFile = metadata.AudioFile;
        MetadataFile = metadata.MetadataFile;
        DirPath = metadata.DirPath;
        BackgroundImageFile = metadata.BackgroundImageFile;
        BackgroundVideoFile = metadata.BackgroundVideoFile;
        VideoGap = metadata.VideoGap;
        Gap = metadata.Gap;
    }

    public TrackAudioSample TrackAudioSample => new(sampleStart, sampleLength);

    // Track Metadata

    public string Artist { get; private set; }
    public string Title { get; private set; }
    public string Creator { get; private set; }
    public int Length { get; private set; }
    public double Bpm { get; private set; }
    public string Version { get; private set; }
    public string AudioFile { get; private set; }
    public string MetadataFile { get; private set; }
    public string DirPath { get; private set; }
    public string? BackgroundImageFile { get; private set; }
    public string? BackgroundVideoFile { get; private set; }
    public double VideoGap { get; private set; }
    public double Gap { get; private set; }

    public override string ToString() => $"{Artist} - {Title}";
}

public struct TrackAudioSample(int length, int start)
{
    public int Start = start;
    public int Length = length;
}
