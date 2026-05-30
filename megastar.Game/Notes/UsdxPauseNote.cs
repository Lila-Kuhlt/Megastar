using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;

namespace megastar.Game.notes;

public class UsdxPauseNote(uint startBeat) : IBeatPaced
{
    public int StartBeat { get; set; }
    public int Length { get; set; }
    public int Pitch { get; set; }
    public string Text { get; set; }
    public UsdxNoteType NoteType { get; set; }

    public Drawable Visual => new SpriteText
    {
        Anchor = Anchor.Centre,
        Origin = Anchor.Centre,
        Text = " ",
    };
}
