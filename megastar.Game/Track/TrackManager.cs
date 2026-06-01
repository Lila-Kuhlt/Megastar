using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using MongoDB.Bson;
using Realms;

namespace megastar.Game.Track;

public class TrackManager(Realm realm)
{
    private readonly Realm realm = realm;

    public MegastarTrackMetadata? trackByObjectId(ObjectId objectId) => realm.Find<MegastarTrackMetadata>(objectId);

    public MegastarTrackMetadata? trackByLastUpdate() =>
        realm.All<MegastarTrackMetadata>().MinBy(track => track.LastVerified);

    public void indexFolder(string path)
    {
        var fileAttributes = File.GetAttributes(path);

        if (!File.Exists(path) || !fileAttributes.HasFlag(FileAttributes.Directory))
            return;

        foreach (var file in Directory.EnumerateFiles(path, "*.txt", SearchOption.AllDirectories))
        {
            var meta = LoadFile(file);
            // TODO: Find duplicate song in DB somehow
        }
    }

    public static MegastarTrackMetadata? LoadFile(string path)
    {
        var dir = Path.GetDirectoryName(path);
        if (dir == null || Directory.Exists(dir)) return null;

        var contents = File.ReadAllText(path);
        var metadata = Parser.ParseUsdxTrackMetadata(contents);

        // convert to megastar format
        var megaMeta = new MegastarTrackMetadata();
        megaMeta.CollectMetadataFrom(metadata);
        megaMeta.CalculateHashes();

        return megaMeta;
    }
}
