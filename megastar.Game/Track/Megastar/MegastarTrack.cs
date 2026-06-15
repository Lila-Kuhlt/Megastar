namespace megastar.Game.Track.Megastar;

public class MegastarTrack(IVerifiableMetadata trackMetadata) : ITrack
{
    public ITrackMetadata Metadata { get; } = trackMetadata;
    public ITrackData TrackData { get; } = new LazyLoadedTrackData(trackMetadata);
}
