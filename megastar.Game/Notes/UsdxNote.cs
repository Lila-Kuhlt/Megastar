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
    public uint StartBeat { get; set; }
    public int Length { get; set; }
    public int Pitch { get; set; }
    public string Text { get; set; }
    public UsdxNoteType NoteType { get; set; }
    public Drawable Visual { get; }

    public UsdxNote(uint startBeat, int length, int pitch, string text, UsdxNoteType noteType)
    {
        StartBeat = startBeat;
        Length = length;
        Pitch = pitch;
        Text = text;
        NoteType = noteType;
        Visual = new Container()
        {
            // 1. Tell the container to fill the entire parent space (the screen)
            RelativeSizeAxes = Axes.Both,
            Size = Vector2.One,

            // Remove Anchor.Centre and Origin.Centre from the parent
            // so it properly aligns to the top-left of the screen by default

            Children = new Drawable[]
            {
                new Box()
                {
                    // The box stays in the center of the screen
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Colour = Colour4.Teal,
                    Size = new Vector2(length * SCALE_FACTOR, SCALE_FACTOR), // Fixed typo: length
                    Y = -pitch * SCALE_FACTOR,
                    X = startBeat * SCALE_FACTOR
                },
                new SpriteText()
                {
                    // Now, BottomCentre means the actual bottom of the screen!
                    Anchor = Anchor.BottomCentre,
                    Origin = Anchor.BottomCentre,
                    Text = text,
                    Colour = Colour4.White,
                    // Y = -40 moves it 40 pixels UP from the bottom edge so it isn't cut off
                    Y = -40
                }
            }
        };
    }
}
