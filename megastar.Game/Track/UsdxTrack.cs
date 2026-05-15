using System.Collections.Generic;
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
    public List<INote> Notes { get; set; }

    public UsdxTrack(UsdxTrackMetadata metadata)
    {
        TrackMetadata = metadata;
    }

    public UsdxTrack(ITrackMetadata trackMetadata, List<INote> notes)
    {
        TrackMetadata = trackMetadata;
        Notes = notes;
    }
}


public sealed partial class UsdxTrackDrawable : CompositeDrawable, IFilterable
{
    private UsdxTrack data { get; }
    public IEnumerable<LocalisableString> FilterTerms => new LocalisableString[] { data.TrackMetadata.Artist, data.TrackMetadata.Title };

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
