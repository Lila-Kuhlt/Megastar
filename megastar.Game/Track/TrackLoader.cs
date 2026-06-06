using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using megastar.Game.Track.Megastar;
using osu.Framework.Logging;

namespace megastar.Game.Track;

public class TrackLoader(TrackRepository repository)
{
    public void IndexFolder(string path)
    {
        Logger.Log($"Indexing {path}");

        if (!Directory.Exists(path))
            return;

        var paths = Directory.EnumerateFiles(path, "*.txt", SearchOption.AllDirectories);

        var tasks = paths
            .Select(LoadFile)
            .Where(trackPath => trackPath != null);

        repository.AddMultiple(tasks!);
    }

    public static Task<MegastarTrackMetadata?> LoadFileAsync(string path) => Task.FromResult(LoadFile(path));

    public static MegastarTrackMetadata? LoadFile(string path)
    {
        var sw = new Stopwatch();
        sw.Start();

        var dir = Path.GetDirectoryName(path);
        if (dir == null || !Directory.Exists(dir)) return null;

        var metadata = UsdxParser.ParseUsdxFile(path);

        if (metadata == null) return null;

        // convert to megastar format
        var megaMeta = new MegastarTrackMetadata(metadata);
        megaMeta.SetHashes();

        var now = sw.Elapsed;
        Logger.Log($"Loaded {megaMeta} in {now.Milliseconds}ms");

        return megaMeta;
    }
}
