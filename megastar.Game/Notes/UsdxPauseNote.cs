using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;

namespace megastar.Game.notes;

public class UsdxPauseNote(uint startBeat) : INote
{
    public uint StartBeat { get; set; }
    public int Length { get; set; }
    public int Pitch { get; set; }
    public string Text { get; set; }
    public UsdxNoteType NoteType { get; set; }

    private Drawable myVisual = new SpriteText
    {
        Anchor = Anchor.Centre,
        Origin = Anchor.Centre,
        Text = "PAUSE",
    };

    public Drawable Visual
    {
        get
        {
            return myVisual;
        }
    }
}
