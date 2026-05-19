using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;

namespace megastar.Game.notes;

public class UsdxPauseNote(uint startBeat) : IBeatPaced
{
    public uint StartBeat { get; set; }
    public int Length { get; set; }
    public int Pitch { get; set; }
    public string Text { get; set; }
    public UsdxNoteType NoteType { get; set; }

    public Drawable Visual
    {
        get
        {
            return new SpriteText
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Text = "PAUSE",
            };
        }
    }
}
