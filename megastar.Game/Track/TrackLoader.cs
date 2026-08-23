using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using megastar.Game.Track.Megastar;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Logging;
using Realms;

namespace megastar.Game.Track;

public class TrackLoader(TrackRepository repository)
{
    /// <summary>
    /// Indexes a given folder. If given a function to be executed "onTrackIndexed", this will be done with the result of the metadata
    /// </summary>
    /// <param name="path">the path to be scanned</param>
    /// <param name="onTrackIndexed">action that is done on with the metadata as argument, e.g. adding to the queue</param>
    public void IndexFolder(string path, System.Action<MegastarTrackMetadata>? onTrackIndexed = null)
    {
        Logger.Log($"Indexing {path}");

        if (!Directory.Exists(path))
            return;

        var paths = Directory.GetDirectories(path)
            .SelectMany(dir => Directory.GetFiles(dir, "*.txt"));

        var loadedTracks = paths
            .Select(LoadFile)
            .Where(metadata => metadata != null)
            .ToList();

        if (loadedTracks.Count == 0) return;

        //Open ONE database connection using the existing Run method and only use one write
        repository.Run(realm =>
        {
            realm.Write(() =>
            {
                foreach (var track in loadedTracks)
                {
                    realm.Add(track!, true);
                }
            });

            // Freezing just creates a copy that is not dependend on a single object and the runtime it got created
            foreach (var track in loadedTracks)
            {
                onTrackIndexed?.Invoke(track!.Freeze());
            }
        });
    }

    public void dropTable()
    {
        repository.Write(realm =>
        {
            realm.RemoveAll<MegastarTrackMetadata>();
        });
    }

    public static Task<MegastarTrackMetadata?> LoadFileAsync(string path) => Task.FromResult(LoadFile(path));

    public static MegastarTrackMetadata? LoadFile(string path)
    {
        var sw = new Stopwatch();
        sw.Start();

        var dir = Path.GetDirectoryName(path);
        if (dir == null || !Directory.Exists(dir)) return null;

        var usdxTrack = UsdxParser.ParseUsdxFile(path);

        if (usdxTrack == null) return null;

        // convert to megastar format
        var megaMeta = new MegastarTrackMetadata(usdxTrack.Metadata);
        megaMeta.SetHashes();

        var now = sw.Elapsed;
        Logger.Log($"Loaded {megaMeta} in {now.Milliseconds}ms");

        return megaMeta;
    }
}
