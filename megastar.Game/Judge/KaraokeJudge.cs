using System;
using megastar.Game.notes;

namespace megastar.Game.Audio;

public class KaraokeJudge
{
    private GameDifficulty difficulty;
    private int maxScorePerNote = 20;
    private int diffFactor = 1;
    private int totalBeats = 1; //init with one so there are no division by 0 errors
    public int Score { get; private set; } = 0;
    public int MaxScore => totalBeats * maxScorePerNote;

    public KaraokeJudge(GameDifficulty difficulty)
    {
        this.difficulty = difficulty;
        switch (difficulty)
        {
            case GameDifficulty.Kuhlant: diffFactor = 1; break;
            case GameDifficulty.Kuhtastrophal: diffFactor = 2; break;
            case GameDifficulty.Muuuuuhtig: diffFactor = 3; break;
        }
    }

    /// <summary>
    /// Adds to the score, based on how far the note is away from the target
    /// </summary>
    /// <param name="sungNote">The note that was sung</param>
    /// <param name="targetNote">The target note</param>
    public void addNoteJudge(INote sungNote, INote targetNote)
    {
        totalBeats++;
        Score += Math.Max((maxScorePerNote - Math.Abs(sungNote.Pitch - targetNote.Pitch) * diffFactor), 0);
    }


}
