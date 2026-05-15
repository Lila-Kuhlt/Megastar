using System.Collections.Generic;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osuTK.Graphics;

namespace megastar.Game;

public partial class UsdxTrack : CompositeDrawable, IFilterable
{
    public ITrackMetadata TrackMetadata { get; set; }

    public IEnumerable<LocalisableString> FilterTerms => new LocalisableString[]
    {
        TrackMetadata.artist,
        TrackMetadata.title
    };

    private bool matchingFilter = true;

    //Fade in/out on match
    public bool MatchingFilter
    {
        get => matchingFilter;
        set
        {
            matchingFilter = value;
            // Smoothly fade the entire drawable in or out based on filter match
            this.FadeTo(value ? 1 : 0, 200);
        }
    }

    public bool FilteringActive { get; set; }

    public UsdxTrack(UsdxTrackMetadata usdxTrackMetadata)
    {
        TrackMetadata = usdxTrackMetadata;

        AutoSizeAxes = Axes.Y;
        RelativeSizeAxes = Axes.X;

        //This is the visual represenatation of the Track
        InternalChild = new SpriteText
        {
            Text = $"{TrackMetadata.title} - {TrackMetadata.artist}",
            Font = FontUsage.Default.With(size: 20),
            Colour = Color4.White,
            Anchor = Anchor.CentreLeft,
            Origin = Anchor.CentreLeft
        };
    }
}
