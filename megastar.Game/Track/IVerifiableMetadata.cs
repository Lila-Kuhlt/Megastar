using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace megastar.Game.Track;

/// <summary>
/// A verifiable TrackMetadata.
/// Uses md5 to generate file hashes of the files of the underlaying ITrackMetadata.
/// Should only be set using <see cref="VerifiableMetadataExtension.fileHash"/> or <see cref="VerifiableMetadataExtension.fileHashAsync"/>
/// When verifying a file hash (comparing the file hash in the IVerifiableMetadata to the current file hash) LastVerified should be updated.
/// This entire process can be done using <see cref="VerifiableMetadataExtension.VerfiyMetadata"/>
/// </summary>
public interface IVerifiableMetadata : ITrackMetadata
{
    byte[]? AudioFileHash { get; set; }
    byte[]? MetadataFileHash { get; set; }
    byte[]? BackgroundImageHash { get; set; }
    byte[]? BackgroundVideoHash { get; set; }

    // Last time the hashes have been verified, so the database can query by lastUpdate to do updates
    DateTimeOffset LastVerified { get; set; }
}

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
