using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace megastar.Game.notes;

public class UsdxNote : INote
{
    public static int SCALE_FACTOR = 10;
    public static int HEIGHT_FACTOR = 15;

    public int StartBeat { get; set; }

    public int Length { get; set; }
    public int Pitch { get; set; }
    public string Text { get; set; }
    public UsdxNoteType NoteType { get; set; }

    public Drawable Visual => CreateVisual();

    public UsdxNote(int startBeat, int length, int pitch, string text, UsdxNoteType noteType)
    {
        StartBeat = startBeat;
        Length = length;
        Pitch = pitch;
        Text = text;
        NoteType = noteType;
    }

    private Drawable CreateVisual()
    {
        Colour4 baseColor;
        Colour4 glowColor;

        switch (NoteType)
        {
            case UsdxNoteType.Golden:
                baseColor = Colour4.Gold;
                glowColor = Colour4.Goldenrod.Opacity(0.8f);
                break;
            case UsdxNoteType.Freestyle:
                baseColor = Colour4.Gray;
                glowColor = Colour4.Transparent;
                break;
            case UsdxNoteType.Sung:
                baseColor = Colour4.LimeGreen;
                glowColor = Colour4.LimeGreen.Opacity(0.8f);
                break;
            default:
                baseColor = Colour4.DeepSkyBlue;
                glowColor = Colour4.DeepSkyBlue.Opacity(0.5f);
                break;
        }

        int finalHeight = NoteType == UsdxNoteType.Sung ? HEIGHT_FACTOR - 4 : HEIGHT_FACTOR;

        float yOffset = NoteType == UsdxNoteType.Sung ? 2 : 0;

        return new Container
        {
            X = this.StartBeat * SCALE_FACTOR,
            Y = -(this.Pitch * HEIGHT_FACTOR) + yOffset,
            Size = new Vector2(this.Length * SCALE_FACTOR, finalHeight),

            Masking = true,
            CornerRadius = finalHeight / 2f,

            BorderColour = Colour4.White,
            BorderThickness = NoteType == UsdxNoteType.Sung ? 0 : 1.5f,

            EdgeEffect = new EdgeEffectParameters
            {
                Type = EdgeEffectType.Glow,
                Colour = glowColor,
                Radius = 8,
                Hollow = true
            },

            Children =
            [
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = baseColor,
                },
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Height = 0.5f,
                    Colour = Colour4.White.Opacity(0.3f),
                }
            ]
        };
    }

    public override string ToString() => Text;
}
