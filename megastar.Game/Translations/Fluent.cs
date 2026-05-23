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

    /// <summary>
    /// A string that works best with a <see cref="FluentTranslationStore"/> as LocalisationStore
    /// </summary>
    public static LocalisableString Translate(string msgWithAttr, params (string, IFluentType)[] args)
    {
        // LocalisableString is the class that is actually translated by the TranslationManager, Fluent just provides the interface to work with the FluentTranslationStore
        return new LocalisableString(new Fluent(msgWithAttr, args));
    }

    public string GetLocalised(LocalisationParameters parameters)
    {
        if (parameters.Store is FluentTranslationStore fluentTranslationStore)
        {
            // Straight up passthrough for fluent/Linguini method
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
        return other != null && key == other.key && values == other.values;
    }

    public bool Equals(ILocalisableStringData other)
    {
        if (other is Fluent fluent)
        {
            return Equals(fluent);
        }
        return false;
    }
}
