using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using MongoDB.Bson;
using Realms;

namespace megastar.Game.Track.Megastar;

public interface IVerifiableMetadata : ITrackMetadata
{
    byte[]? AudioFileHash { get; set; }
    byte[]? MetadataFileHash { get; set; }
    byte[]? BackgroundImageHash { get; set; }
    byte[]? BackgroundVideoHash { get; set; }

    // Last time the hashes have been verified, so the database can query by lastUpdate to do updates
    DateTimeOffset LastVerified { get; set; }
}

public partial class MegastarTrackMetadata : IVerifiableMetadata, IRealmObject
{
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

    public TrackAudioSample TrackAudioSample => new(sampleStart, sampleLength);

    public MegastarTrack ToMegastarTrack() => new(this);

    // Track Metadata

    public string Artist { get; }
    public string Title { get; }
    public string Creator { get; }
    public int Length { get; }
    public double Bpm { get; }
    public string Version { get; }
    public string AudioFile { get; }
    public string MetadataFile { get; }
    public string DirPath { get; }
    public string? BackgroundImageFile { get; }
    public string? BackgroundVideoFile { get; }
    public double VideoGap { get; }
    public double Gap { get; }

    public override string ToString() => $"{Artist} - {Title}";
}

public struct TrackAudioSample(int length, int start)
{
    public int Start = start;
    public int Length = length;
}

// Might want to extract this to a class
public static class VerifiableMetadataExtension
{
    /// <summary>
    /// Checks if files have changed since last index by comparing md5 hashes of provided files.
    ///
    /// DOES NOT RE-READ METADATA AND CHECKS IF A NEW FILE IS PRESENT.
    /// This means that if the metadata changed (which it will detect) the file has to re-read (aka re-index it)
    /// </summary>
    /// <param name="metadata">The metadata to verify</param>
    /// <returns>Whether the metadata matches the files in the directory</returns>
    public static bool VerfiyMetadata(this IVerifiableMetadata metadata)
    {
        if (metadata.MetadataFileHash != fileHash(metadata.MetadataFilePath())) return false;
        if (metadata.AudioFileHash != fileHash(metadata.AudioFilePath())) return false;

        var imageFilePath = metadata.BackgroundImageFilePath();
        if (imageFilePath != null && metadata.BackgroundImageHash != fileHash(imageFilePath)) return false;

        var videoFilePath = metadata.BackgroundVideoFilePath();
        if (videoFilePath != null && metadata.BackgroundImageHash != fileHash(videoFilePath)) return false;

        metadata.LastVerified = DateTimeOffset.Now;

        return true;
    }

    public static void SetHashes(this IVerifiableMetadata metadata)
    {
        metadata.MetadataFileHash = fileHash(metadata.MetadataFilePath());
        metadata.AudioFileHash = fileHash(metadata.AudioFilePath());
        metadata.BackgroundImageHash = fileHash(metadata.BackgroundImageFilePath());
        metadata.BackgroundVideoHash = fileHash(metadata.BackgroundVideoFilePath());

        metadata.LastVerified = DateTimeOffset.Now;
    }

    public static async Task SetHashesAsync(this IVerifiableMetadata metadata)
    {
        // spawn hash tasks
        IEnumerable<Task> tasks =
        [
            fileHashAsync(metadata.MetadataFilePath())
                .ContinueWith(async hash => metadata.MetadataFileHash = await hash),
            fileHashAsync(metadata.AudioFilePath())
                .ContinueWith(async hash => metadata.AudioFileHash = await hash),
            fileHashAsync(metadata.BackgroundImageFilePath())
                .ContinueWith(async hash => metadata.BackgroundImageHash = await hash),
            fileHashAsync(metadata.BackgroundVideoFilePath())
                .ContinueWith(async hash => metadata.BackgroundVideoHash = await hash)
        ];

        await Task.WhenAll(tasks);
        metadata.LastVerified = DateTimeOffset.Now;
    }

    private static byte[]? fileHash(string? path)
    {
        if (path == null || !File.Exists(path))
            return null;

        return MD5.HashData(File.ReadAllBytes(path));
    }

    private static async Task<byte[]?> fileHashAsync(string? path)
    {
        if (path == null || !File.Exists(path))
            return null;

        await using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        return await MD5.HashDataAsync(stream);
    }
}
