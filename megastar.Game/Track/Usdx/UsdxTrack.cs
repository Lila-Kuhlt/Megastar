namespace megastar.Game.Track.Usdx;

public class UsdxTrack(UsdxTrackMetadata metadata) : ITrack
{
    public ITrackMetadata Metadata { get; } = metadata;
    public ITrackData TrackData { get; } = new LazyLoadedTrackData(metadata);
}
