using System.Collections.Generic;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osuTK.Graphics;

namespace megastar.Game.Track.Usdx;

public sealed partial class UsdxTrackDrawable : CompositeDrawable, IFilterable
{
    private ITrackMetadata metadata { get; }

    public IEnumerable<LocalisableString> FilterTerms => new LocalisableString[]
        { metadata.Artist, metadata.Title };

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

    public UsdxTrackDrawable(ITrackMetadata metadata)
    {
        this.metadata = metadata;
        AutoSizeAxes = Axes.Y;
        RelativeSizeAxes = Axes.X;

        InternalChild = new SpriteText
        {
            Text = $"{this.metadata.Title} - {this.metadata.Artist}",
            Font = FontUsage.Default.With(size: 20),
            Colour = Color4.White
        };
    }
}
