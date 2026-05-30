using JetBrains.Annotations;
using MongoDB.Bson;
using Realms;

namespace megastar.Game.Track;

public partial class MegastarTrack : IRealmObject, ITrackMetadata
{
    [MapTo("_id")]
    [PrimaryKey]
    public ObjectId Id { get; private set; } = ObjectId.GenerateNewId(); // TODO: Get Id from USDX, OR generate new

    [CanBeNull] public byte[] SongFileHash { get; set; }
    [CanBeNull] public byte[] IndexFileHash { get; set; }
    [CanBeNull] public byte[] BackgroundImageHash { get; set; }
    [CanBeNull] public byte[] BackgroundVideoHash { get; set; }

    private int sampleStart;
    private int sampleLength;

    [Ignored] public TrackAudioSample TrackAudioSample => new(sampleStart, sampleLength);

    // Track Metadata

    public string Artist { get; set; }
    public string Title { get; set; }
    public string Creator { get; set; }
    public int Length { get; set; }
    public double Bpm { get; set; }
    public string Version { get; set; }
    public string SongFile { get; set; }
    public string Path { get; set; }
    public string DirPath { get; set; }
    public string BackgroundImageFile { get; set; }
    public string BackgroundVideoFile { get; set; }
    public double VideoGap { get; set; }
    public double Gap { get; set; }
}

public struct TrackAudioSample(int length, int start)
{
    public int Start = start;
    public int Length = length;
}
