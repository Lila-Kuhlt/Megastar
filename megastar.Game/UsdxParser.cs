using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using megastar.Game.notes;
using megastar.Game.Track.Usdx;

namespace megastar.Game;

public static class UsdxParser
{
    public static UsdxTrack? ParseUsdxFile(string manifestPath)
    {
        Dictionary<string, string> metadata = new Dictionary<string, string>();

        if (!File.Exists(manifestPath)) return null;

        foreach (var line in File.ReadLines(manifestPath))
        {
            if (line.StartsWith('#'))
            {
                metadata.Add(line.Split(":")[0].Replace("#", "").ToLower(), line.Split(":")[1]);
            }
            else if (line.StartsWith('E') || line.StartsWith('P'))
            {
                break;
            }
        }

        var usdx = extractMetadata(metadata, manifestPath);
        return usdx == null ? null : new UsdxTrack(usdx);
    }

    private static UsdxTrackMetadata? extractMetadata(Dictionary<string, string> trackMetadata, string metadataFile)
    {
        trackMetadata.TryGetValue("artist", out var artist);
        trackMetadata.TryGetValue("title", out var title);
        trackMetadata.TryGetValue("creator", out var creator);
        trackMetadata.TryGetValue("length", out var slength);
        trackMetadata.TryGetValue("bpm", out var sbpm);
        trackMetadata.TryGetValue("version", out var version);
        trackMetadata.TryGetValue("mp3", out var mp3);
        trackMetadata.TryGetValue("audio", out var audio);
        trackMetadata.TryGetValue("video", out var video);
        trackMetadata.TryGetValue("background", out var background);
        trackMetadata.TryGetValue("gap", out var sgap);

        var length = Convert.ToInt32(slength);
        var bpm = Convert.ToDouble(sbpm, CultureInfo.InvariantCulture);
        var gap = Convert.ToDouble(sgap, CultureInfo.InvariantCulture);

        var audioFile = audio ?? mp3;
        var dirPath = Path.GetDirectoryName(metadataFile)!;

        if (audioFile == null) return null;
        if (title == null) return null;
        if (artist == null) return null;

        return new UsdxTrackMetadata(metadataFile: metadataFile, dirPath: dirPath,
            artist: artist, title: title, creator: creator ?? "?", audioFile: audioFile, length: length,
            backgroundImageFile: background, backgroundVideoFile: video, bpm: bpm, gap: gap, version: version ?? "?",
            videoGap: gap);
    }

    public static List<IBeatPaced> ParseUsdxNotes(string rawUsdx)
    {
        List<IBeatPaced> notes = [];

        using var reader = new StringReader(rawUsdx);
        while (reader.ReadLine() is { } line)
        {
            if (line.StartsWith('#')) continue;
            if (line.StartsWith('E') || line.StartsWith('P')) break;

            notes.Add(ParseUsdxNote(line));
        }

        return notes;
    }

    [Obsolete("Use ParseUsdxFile instead", true)]
    public static UsdxTrackMetadata ParseUsdxTrackMetadata(string rawUsdx)
    {
        if (!rawUsdx.StartsWith('#'))
        {
            throw new InvalidDataException();
        }

        Dictionary<string, string> metadata = new Dictionary<string, string>();
        using var reader = new StringReader(rawUsdx);

        while (reader.ReadLine() is { } line)
        {
            if (!line.StartsWith('#'))
            {
                break;
            }

            // TODO Was getting OutOfBounds (most likely with invalid files), maybe console warning or not parse these files.
            if (line.Split(":").Length >= 2)
                metadata.Add(line.Split(":")[0].Replace("#", "").ToLower(), line.Split(":")[1]);
        }

        return extractMetadata(metadata, "")!;
    }

    public static IBeatPaced ParseUsdxNote(string line)
    {
        string[] splitNote = line.Split(" ");

        if (splitNote[0].Equals("-"))
        {
            return new UsdxPauseNote(Convert.ToUInt32(splitNote[1]));
        }

        UsdxNoteType noteType = splitNote[0] switch
        {
            ":" => UsdxNoteType.Normal,
            "*" => UsdxNoteType.Golden,
            "F" => UsdxNoteType.Freestyle,
            "R" => UsdxNoteType.Rap,
            "G" => UsdxNoteType.Golden,
            _ => UsdxNoteType.Normal
        };

        int startBeat = Convert.ToInt32(splitNote[1]);
        int lenght = Convert.ToInt32(splitNote[2]);
        int pitch = Convert.ToInt32(splitNote[3]);
        string text = splitNote[4];

        return new UsdxNote(startBeat, lenght, pitch, text, noteType);
    }
}
