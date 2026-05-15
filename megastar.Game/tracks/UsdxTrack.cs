namespace megastar.Game;

public class UsdxTrack(UsdxTrackMetadata usdxTrackMetadata) : ITrack
{
    public ITrackMetadata trackMetadata { get => usdxTrackMetadata; set => value = usdxTrackMetadata; }
}
