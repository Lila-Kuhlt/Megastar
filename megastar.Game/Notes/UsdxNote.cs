using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace megastar.Game.notes;

public class UsdxNote
    : INote
{
    public static int SCALE_FACTOR = 10;
    public uint StartBeat { get; set; }
    public int Length { get; set; }
    public int Pitch { get; set; }
    public string Text { get; set; }
    public UsdxNoteType NoteType { get; set; }
    public Drawable Visual { get; }

    public UsdxNote(uint startBeat, int lenght, int pitch, string text, UsdxNoteType noteType)
    {
        StartBeat = startBeat;
        Length = lenght;
        Pitch = pitch;
        Text = text;
        NoteType = noteType;
        Visual = new Container()
        {
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Children = new Drawable[]
            {
                new Box()
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Colour = Colour4.Teal,
                    Size = new Vector2(lenght * SCALE_FACTOR, SCALE_FACTOR),
                    Y = -pitch * SCALE_FACTOR,
                    X = startBeat * SCALE_FACTOR
                }
            }
        };
    }
}
