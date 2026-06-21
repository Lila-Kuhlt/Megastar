namespace megastar.Game.Track;

public interface ITrack
{
    ITrackMetadata Metadata { get; }
    ITrackData TrackData { get; }
}
