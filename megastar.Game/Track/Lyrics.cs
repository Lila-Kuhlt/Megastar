using System.Collections;
using System.Collections.Generic;
using System.Linq;
using megastar.Game.notes;

namespace megastar.Game.Track;

/// <summary>
/// A single lyric -- a single line in the entire lyrics
/// </summary>
public class Lyric
{
    public readonly List<INote> Notes = [];
    public readonly string Text;

    public int StartBeat => Notes[0].StartBeat;
    public int EndBeat => Notes[^1].StartBeat + Notes[^1].Length;
    public int TotalLength => EndBeat - StartBeat;

    public Lyric(List<IBeatPaced> notes)
    {
        string wordAggregator = string.Empty;

        foreach (var note in notes)
        {
            switch (note)
            {
                case UsdxNote usdxNote:
                    // might want to dynamically detect word gaps
                    wordAggregator += usdxNote.Text;
                    Notes.Add(usdxNote);
                    continue;
                case UsdxPauseNote _: // should not happen
                    continue;
            }
        }

        Text = wordAggregator;
    }
}

public class Lyrics(List<Lyric> lyrics)
{
    // using lines as name because Lyrics is forbidden :/ also no red black trees in c# :(
    public readonly List<Lyric> Lines = lyrics;

    public Lyrics(UsdxTrack usdxTrack) : this(usdxTrack.LyricEnumerator().ToList())
    {
    }

    public Lyric? LyricForBeat(int beat) => Lines.Find(x => x.StartBeat <= beat && beat < x.EndBeat);

    // Fetches the Lyric after the current one, which is determined by the beat
    public Lyric? LyricAfterBeat(int beat)
    {
        var i = LyricIndexForBeat(beat) + 1;
        return i >= 0 && i < Lines.Count ? Lines[i] : null;
    }

    public int LyricIndexForBeat(int beat) => Lines.FindIndex(x => x.StartBeat < beat && beat < x.EndBeat);
}

public record LyricEnumerator(ITrackData Data) : IEnumerable<Lyric>
{
    public IEnumerator<Lyric> GetEnumerator()
    {
        List<IBeatPaced> aggregator = [];
        foreach (var note in Data.Notes.Value)
        {
            switch (note)
            {
                case UsdxNote usdxNote:
                    aggregator.Add(usdxNote);
                    continue;
                case UsdxPauseNote _:
                    yield return new Lyric(aggregator);
                    aggregator = [];
                    continue;
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public static class TrackDataLyricExtensions
{
    public static IEnumerable<Lyric> LyricEnumerator(this ITrackData data) => new LyricEnumerator(data);
}
