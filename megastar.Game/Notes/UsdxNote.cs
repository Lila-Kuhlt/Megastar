using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace megastar.Game.notes;

public class UsdxNote
    : INote
{
    public static int SCALE_FACTOR = 10;
    public static int HEIGHT_FACTOR = 10;
    public uint StartBeat { get; set; }
    public int Length { get; set; }
    public int Pitch { get; set; }
    public string Text { get; set; }
    public UsdxNoteType NoteType { get; set; }

    public Drawable Visual
    {
        get
        {
            return new Container
            {
                AutoSizeAxes = Axes.Both,
                X = this.StartBeat * SCALE_FACTOR,

                Children = new Drawable[]
                {
                    new Box
                    {
                        Colour = Colour4.Teal,
                        Size = new Vector2(this.Length * SCALE_FACTOR, HEIGHT_FACTOR),
                        Y = Pitch * HEIGHT_FACTOR
                    },
                    new SpriteText
                    {
                        Text = this.Text,
                        Colour = Colour4.White,
                        Anchor = Anchor.BottomCentre,
                        Origin = Anchor.TopCentre,
                        Y = 40
                    }
                }
            };
        }
    }

    public UsdxNote(uint startBeat, int length, int pitch, string text, UsdxNoteType noteType)
    {
        StartBeat = startBeat;
        Length = length;
        Pitch = pitch;
        Text = text;
        NoteType = noteType;
    }
}
