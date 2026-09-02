using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using megastar.Game.notes;
using megastar.Game.Track.Usdx;
using osu.Framework.Extensions.ObjectExtensions;
using osu.Framework.Graphics;

namespace megastar.Game;

public static class UsdxParser
{

    private static Colour4[] playerColours = { Colour4.DeepSkyBlue, Colour4.DarkBlue };
    private static int endLastBeat = 0;
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
        trackMetadata.TryGetValue("cover", out var cover);
        trackMetadata.TryGetValue("gap", out var sgap);

        var length = Convert.ToInt32(slength);
        var bpm = Convert.ToDouble(sbpm, CultureInfo.InvariantCulture);
        var gap = Convert.ToDouble(sgap, CultureInfo.InvariantCulture);

        var audioFile = audio ?? mp3;
        var dirPath = Path.GetDirectoryName(metadataFile)!;
        var manifest = Path.GetFileName(metadataFile);

        if (audioFile == null) return null;
        if (title == null) return null;
        if (artist == null) return null;

        if (dirPath.ToLower().Contains("duet"))
        {
            title += " - Duet";
        }

        return new UsdxTrackMetadata(MetadataFile: manifest, DirPath: dirPath,
            Artist: artist, Title: title, Creator: creator ?? "?", AudioFile: audioFile, Length: length,
            BackgroundImageFile: background.IsNotNull() ? background : cover, BackgroundVideoFile: video, Bpm: bpm, Gap: gap, Version: version ?? "?",
            VideoGap: gap);
    }

    public static List<IBeatPaced> ParseUsdxNotes(string rawUsdx)
    {
        bool player1Active = true;
        List<IBeatPaced> notes = [];


        using var reader = new StringReader(rawUsdx);
        while (reader.ReadLine() is { } line)
        {
            if (line.StartsWith("P1")) continue;
            if (line.StartsWith("P2")) { player1Active = false; continue; }
            if (line.StartsWith('#')) continue;
            if (line.StartsWith('E') || line.StartsWith('P')) break;

            notes.Add(parseUsdxNote(line, player1Active ? 0 : 1));
        }

        if (!player1Active)
        {
            //This is for fixing up duets, orderBy uses stable sort
            notes = notes.OrderBy(n => n.StartBeat).ToList();
            bool lastNotePause = true;
            string lastText = "";
            List<IBeatPaced> doubleNotes = new List<IBeatPaced>();
            foreach (var note in notes)
            {
                if (note.GetType() == typeof(UsdxPauseNote))
                {
                    if (lastNotePause)
                    {
                        doubleNotes.Add(note);
                    }
                    lastNotePause = true;
                }
                else
                {
                    lastNotePause = false;
                }

                if (note.Text.Equals(lastText) && !note.Text.Equals(""))
                {
                    doubleNotes.Add(note);
                }
                lastText = note.Text;
            }

            foreach (var doublePauseNote in doubleNotes)
            {
                notes.Remove(doublePauseNote);
            }
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

    private static IBeatPaced parseUsdxNote(string line, int player1ActiveColourIndex = 0)
    {
        string[] splitNote = line.Split(" ");

        if (splitNote[0].Equals("-"))
        {
            //For doing duets, the pause is messed up, when not using the end of the phrase
            return new UsdxPauseNote(Math.Min(Convert.ToInt32(splitNote[1]), endLastBeat));
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
        endLastBeat = startBeat + lenght;

        return new UsdxNote(startBeat, lenght, pitch, text, noteType, playerColours[player1ActiveColourIndex]);
    }
}
