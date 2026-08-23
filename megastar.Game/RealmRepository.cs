using System;
using System.Threading.Tasks;
using osu.Framework.Development;
using Realms;

namespace megastar.Game;

public class RealmRepository(RealmConfigurationBase realmConfiguration) : IDisposable
{
    private Realm Realm => ensureUpdateRealm();
    private Realm? updateRealm;

    public RealmRepository(string identifier) : this(new RealmConfiguration(identifier)) { }

    private Realm threadedRealmContext()
    {
        try
        {
            // Try to open the database normally
            return Realm.GetInstance(realmConfiguration);
        }
        catch (Realms.Exceptions.RealmMigrationNeededException)
        {
            // If the schema changed, delete the old file and create a new one
            Realm.DeleteRealm(realmConfiguration);
            return Realm.GetInstance(realmConfiguration);
        }
    }

    private Realm ensureUpdateRealm()
    {
        if (!ThreadSafety.IsUpdateThread)
            throw new InvalidOperationException("Tried to initialize RealmRepository from non-update thread");

        if (updateRealm != null) return updateRealm;

        var realm = threadedRealmContext();
        updateRealm = realm;
        return realm;
    }

    public void Run(Action<Realm> action)
    {
        if (ThreadSafety.IsUpdateThread)
        {
            action(Realm);
            return;
        }

        using var realm = threadedRealmContext();
        action(realm);
    }

    public T Run<T>(Func<Realm, T> func)
    {
        if (ThreadSafety.IsUpdateThread)
            return func(Realm);

        using var realmCtx = threadedRealmContext();
        return func(realmCtx);
    }

    public void Write(Action<Realm> action) => Run(r => r.Write(() => action(r)));
    public async Task WriteAsync(Action<Realm> action) => await Run(r => r.WriteAsync(() => action(r)));

    public void Dispose()
    {
        updateRealm?.Dispose();
    }
}
