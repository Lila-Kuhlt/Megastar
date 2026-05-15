namespace megastar.Game;

public class UsdxTrack : ITrack
{
    // Auto-property handles both getting and setting automatically
    public ITrackMetadata trackMetadata { get; set; }

    // Traditional constructor
    public UsdxTrack(UsdxTrackMetadata usdxTrackMetadata)
    {
        trackMetadata = usdxTrackMetadata;
    }
}
