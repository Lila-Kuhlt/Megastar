using System;
using System.Collections.Generic;
using System.IO;
using megastar.Game.notes;
using megastar.Game.Track;

namespace megastar.Game;

public class Parser
{
    public static UsdxTrack ParseUsdxFile(string rawUsdx)
    {
        Dictionary<string, string> metadata = new Dictionary<string, string>();
        List<IBeatPaced> notes = new List<IBeatPaced>();

        using var reader = new StringReader(rawUsdx);
        while (reader.ReadLine() is { } line)
        {
            if (line.StartsWith('#'))
            {
                metadata.Add(line.Split(":")[0].Replace("#", "").ToLower(), line.Split(":")[1]);
            }
            else if (line.StartsWith('E') || line.StartsWith('P'))
            {
                break;
            }
            else
            {
                notes.Add(ParseUsdxNote(line));
            }
        }

        UsdxTrackMetadata trackMetadata = new UsdxTrackMetadata(metadata);
        return new UsdxTrack(trackMetadata, notes);
    }

    public static List<IBeatPaced> ParseUsdxNotes(string rawUsdx)
    {
        List<IBeatPaced> notes = new List<IBeatPaced>();

        using var reader = new StringReader(rawUsdx);
        while (reader.ReadLine() is { } line)
        {
            if (line.StartsWith('#'))
            {
                continue;
            }
            else if (line.StartsWith('E') || line.StartsWith('P'))
            {
                break;
            }
            else
            {
                notes.Add(ParseUsdxNote(line));
            }
        }

        return notes;
    }

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

            metadata.Add(line.Split(":")[0].Replace("#", "").ToLower(), line.Split(":")[1]);
        }

        return new UsdxTrackMetadata(metadata);
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

        uint startBeat = Convert.ToUInt32(splitNote[1]);
        int lenght = Convert.ToInt32(splitNote[2]);
        int pitch = Convert.ToInt32(splitNote[3]);
        string text = splitNote[4];

        return new UsdxNote(startBeat, lenght, pitch, text, noteType);
    }
}
