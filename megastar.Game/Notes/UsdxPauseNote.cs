using System;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;

namespace megastar.Game.notes;

public class UsdxPauseNote(uint startBeat) : IBeatPaced
{
    public int StartBeat { get; }
    public int Length { get; set; }
    public int Pitch { get; set; }
    public string Text { get; set; } = string.Empty;
    public UsdxNoteType NoteType { get; set; } = UsdxNoteType.Freestyle;

    public Drawable Visual => new SpriteText
    {
        Anchor = Anchor.Centre,
        Origin = Anchor.Centre,
        Text = " ",
    };
}
