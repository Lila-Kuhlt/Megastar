using System.Collections.Generic;
using System.IO;
using megastar.Game.notes;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osuTK.Graphics;

namespace megastar.Game.Track;

public class UsdxTrack : ITrack
{
    public ITrackMetadata TrackMetadata { get; set; }
    private List<IBeatPaced> notes;
    public List<IBeatPaced> Notes
    {
        get
        {
            // If notes haven't been loaded yet and we have a valid metadata path, parse them
            if (notes == null && !string.IsNullOrEmpty(TrackMetadata?.path))
            {
                if (File.Exists(TrackMetadata.path))
                {
                    string rawString = File.ReadAllText(TrackMetadata.path);
                    notes = Parser.ParseUsdxNotes(rawString);
                }
                else
                {
                    // Fallback to an empty list if the file doesn't exist to prevent repeated crashes
                    notes = new List<IBeatPaced>();
                }
            }
            return notes;
        }
        set => notes = value;
    }

    public UsdxTrack(UsdxTrackMetadata metadata)
    {
        TrackMetadata = metadata;
    }



    public UsdxTrack(ITrackMetadata trackMetadata, List<IBeatPaced> notes)
    {
        TrackMetadata = trackMetadata;
        Notes = notes;
    }
}

public sealed partial class UsdxTrackDrawable : CompositeDrawable, IFilterable
{
    private UsdxTrack data { get; }

    public IEnumerable<LocalisableString> FilterTerms => new LocalisableString[]
        { data.TrackMetadata.Artist, data.TrackMetadata.Title };

    private bool matchingFilter = true;

    public bool MatchingFilter
    {
        get => matchingFilter;
        set
        {
            matchingFilter = value;
            this.FadeTo(value ? 1 : 0, 200);
        }
    }

    public bool FilteringActive { get; set; }

    public UsdxTrackDrawable(UsdxTrack data)
    {
        this.data = data;
        AutoSizeAxes = Axes.Y;
        RelativeSizeAxes = Axes.X;

        InternalChild = new SpriteText
        {
            Text = $"{this.data.TrackMetadata.Title} - {this.data.TrackMetadata.Artist}",
            Font = FontUsage.Default.With(size: 20),
            Colour = Color4.White
        };
    }
}
