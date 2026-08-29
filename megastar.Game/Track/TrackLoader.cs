using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
    private int MAX_BATCH_SIZE = 1000;
    /// <summary>
    /// Indexes a given folder using a Producer-Consumer pipeline for maximum throughput.
    /// </summary>
    public void IndexFolder(string path, System.Action<MegastarTrackMetadata>? onTrackIndexed = null)
    {
        Logger.Log($"Indexing {path}");

        if (!Directory.Exists(path))
            return;

        // thread-safe queue
        using var queue = new BlockingCollection<MegastarTrackMetadata>(5 * MAX_BATCH_SIZE);

        // Consumer (Database Writer) on a background thread
        var dbWriterTask = Task.Run(() =>
        {
            repository.Run(realm =>
            {
                var batch = new List<MegastarTrackMetadata>(MAX_BATCH_SIZE);

                // GetConsumingEnumerable blocks and waits for items until CompleteAdding() is called
                foreach (var track in queue.GetConsumingEnumerable())
                {
                    batch.Add(track);

                    // Write in chunks to keep transactions fast and memory low
                    if (batch.Count >= 1000)
                    {
                        CommitBatch(realm, batch, onTrackIndexed);
                        batch.Clear();
                    }
                }

                // Commit any leftovers after the queue is finished
                if (batch.Count > 0)
                {
                    CommitBatch(realm, batch, onTrackIndexed);
                }
            });
        });

        // Producers (File Parsers)
        var paths = Directory.EnumerateDirectories(path)
            .SelectMany(dir => Directory.EnumerateFiles(dir, "*.txt"));

        Parallel.ForEach(paths, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, file =>
        {
            var track = LoadFile(file);
            if (track != null)
            {
                queue.Add(track);
            }
        });


        queue.CompleteAdding();
        dbWriterTask.Wait();
    }

    ///Helper method to handle the Realm transaction and callbacks
    private void CommitBatch(Realm realm, List<MegastarTrackMetadata> batch, System.Action<MegastarTrackMetadata>? onTrackIndexed)
    {
        realm.Write(() =>
        {
            foreach (var track in batch)
            {
                realm.Add(track, true);
            }
        });

        foreach (var track in batch)
        {
            //Copy its values and not the actual object
            onTrackIndexed?.Invoke(track.Freeze());
        }
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
