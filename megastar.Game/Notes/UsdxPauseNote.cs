using System;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;

namespace megastar.Game.notes;

public class UsdxPauseNote(int startBeat) : IBeatPaced
{
    public int StartBeat { get; } = startBeat;
    public int Length { get; set; }
    public int Pitch { get; set; }
    public string Text { get; set; } = string.Empty;
    public UsdxNoteType NoteType { get; set; } = UsdxNoteType.Freestyle;

    public Drawable get_visual(float? scaleFactor) => new SpriteText
    {
        Anchor = Anchor.Centre,
        Origin = Anchor.Centre,
        Text = " ",
    };
}
