using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using osu.Framework.IO.Stores;

namespace megastar.Game;

public class TranslationStore : IResourceStore<StreamReader>
{
    private readonly IResourceStore<byte[]> baseStore;

    public TranslationStore()
    {
        var resourceAssembly = typeof(MegastarGame).Assembly;
        baseStore = new NamespacedResourceStore<byte[]>(new DllResourceStore(resourceAssembly), "Resources");
    }

    public StreamReader Get(string name)
    {
        if (!name.EndsWith(".ftl"))
            name += ".ftl";

        var data = baseStore.Get(name);
        return data != null ? new StreamReader(new MemoryStream(data)) : null;
    }

    public async Task<StreamReader> GetAsync(string name, CancellationToken cancellationToken = default)
    {
        if (!name.EndsWith(".ftl"))
            name += ".ftl";

        var data = await baseStore.GetAsync(name, cancellationToken);
        return data != null ? new StreamReader(new MemoryStream(data)) : null;
    }

    public Stream GetStream(string name) => baseStore.GetStream(name);

    public IEnumerable<string> GetAvailableResources() => baseStore.GetAvailableResources();

    public void Dispose() => baseStore?.Dispose();
}
