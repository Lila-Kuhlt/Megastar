using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace megastar.Game.notes;

public class UsdxNote : INote
{
    public static int SCALE_FACTOR = 10;

    // Increased slightly so the notes are thicker and easier to see/hit
    public static int HEIGHT_FACTOR = 15;

    public uint StartBeat { get; set; }
    public int Length { get; set; }
    public int Pitch { get; set; }
    public string Text { get; set; }
    public UsdxNoteType NoteType { get; set; }

    // Cache the visual so we don't accidentally create thousands of containers
    private Drawable cachedVisual;
    public Drawable Visual => cachedVisual ??= CreateVisual();

    public UsdxNote(uint startBeat, int length, int pitch, string text, UsdxNoteType noteType)
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

        // Customize these based on your actual UsdxNoteType enum values!
        switch (NoteType)
        {
            case UsdxNoteType.Golden:
                baseColor = Colour4.Gold;
                glowColor = Colour4.Goldenrod.Opacity(0.8f);
                break;
            case UsdxNoteType.Freestyle: // Spoken/Rap notes usually don't have a specific pitch
                baseColor = Colour4.Gray;
                glowColor = Colour4.Transparent;
                break;
            default:
                baseColor = Colour4.Pink;
                glowColor = Colour4.DeepPink.Opacity(0.5f);
                break;
        }

        return new Container
        {
            X = this.StartBeat * SCALE_FACTOR,
            Y = this.Pitch * HEIGHT_FACTOR,
            Size = new Vector2(this.Length * SCALE_FACTOR, HEIGHT_FACTOR),


            Masking = true,
            CornerRadius = HEIGHT_FACTOR / 2f,
            BorderColour = Colour4.White,
            BorderThickness = 1.5f,

            // Outer glow effect
            EdgeEffect = new EdgeEffectParameters
            {
                Type = EdgeEffectType.Glow,
                Colour = glowColor,
                Radius = 8,
                Hollow = true
            },

            Children = new Drawable[]
            {
                // Base note color
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = baseColor,
                },
                // Top half highlight to give a subtle 3D "glass" or glossy effect
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Height = 0.5f, // Only cover the top half
                    Colour = Colour4.White.Opacity(0.3f),
                }
            }
        };
    }

    public override string ToString()
    {
        return this.Text;
    }
}
