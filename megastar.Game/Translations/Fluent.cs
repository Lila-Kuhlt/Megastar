using System;
using System.Runtime.CompilerServices;
using Linguini.Shared.Types.Bundle;
using osu.Framework.Extensions.ObjectExtensions;
using osu.Framework.Localisation;
using osu.Framework.Logging;

namespace megastar.Game.Translations;

public class Fluent : IEquatable<Fluent>, ILocalisableStringData
{
    private readonly string key;
    private readonly (string, IFluentType)[] values;

    private Fluent(string msgWithAttr, params (string, IFluentType)[] args)
    {
        key = msgWithAttr;
        values = args;
    }

    public static LocalisableString GetString(string msgWithAttr, params (string, IFluentType)[] args)
    {
        return new LocalisableString(new Fluent(msgWithAttr, args));
    }

    public string GetLocalised(LocalisationParameters parameters)
    {
        if (parameters.Store is FluentTranslationStore fluentTranslationStore)
        {
            return fluentTranslationStore.GetAttrMessage(key, values);
        }

        if (parameters.Store.IsNull())
        {
            return key;
        }

        Logger.Log("[WARNING] Using Fluent with LocalisationStores other than FluentTranslationStores is only partially supported.");
        return parameters.Store.Get(key);
    }

    public bool Equals(Fluent other)
    {
        throw new NotImplementedException();
    }

    public bool Equals(ILocalisableStringData other)
    {
        throw new NotImplementedException();
    }
}
