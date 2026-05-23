using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Linguini.Bundle.Builder;
using Linguini.Bundle;
using Linguini.Bundle.Errors;
using Linguini.Shared.Types.Bundle;
using megastar.Resources;
using osu.Framework.Bindables;
using osu.Framework.IO.Stores;
using osu.Framework.Localisation;
using osu.Framework.Logging;

namespace megastar.Game.Translations;

/// <summary>
/// Megastar Translation Store (not TranslationStore because of naming conflicts with framework internal translation handler)
/// </summary>
public class FluentTranslationStore : ILocalisationStore
{
    private readonly IResourceStore<byte[]> baseStore;
    private readonly FluentBundle fluentBundle;

    public string Get(string name)
    {
        return GetAttrMessage(name);
    }

    public Task<string> GetAsync(string name, CancellationToken cancellationToken = new CancellationToken())
    {
        return new  Task<string>(() => Get(name), cancellationToken);
    }


    public FluentTranslationStore(IResourceStore<byte[]> baseStore, string language)
    {
        if (!language.EndsWith(".ftl"))
            language += ".ftl";
        var data = baseStore.Get(language);
        this.baseStore = baseStore;

        var (bundle, errors) = LinguiniBuilder.Builder().CultureInfo(new CultureInfo(Regex.Replace(language, "\\.ftl$", ""))).AddResource(new StreamReader(new MemoryStream(data))).Build();

        if (errors != null && errors.Any())
        {
            errors.ForEach(e => Console.Error.WriteLine(e));
        }

        fluentBundle = bundle;
    }

    /// <summary>
    ///     Passthrough method for <see cref="IReadBundle.GetAttrMessage" />, so there is less method chaining.
    /// </summary>
    /// <param name="msgWithAttr">The message with attribute to retrieve.</param>
    /// <param name="args">Optional arguments to format the message.</param>
    /// <returns>The attribute message from the read bundle.</returns>
    public string GetAttrMessage(string msgWithAttr, params (string, IFluentType)[] args)
    {
        try
        {
            return fluentBundle.GetAttrMessage(msgWithAttr, args);
        }
        catch (LinguiniException e)
        {
            Logger.Error(e, $"[ERROR] Missing translation for key \"{msgWithAttr}\" in language {EffectiveCulture.Name}");
            return null;
        }
    }

    public Stream GetStream(string name) => baseStore.GetStream(name);

    /// <summary>
    /// Returns a list of all available languages by language code.
    /// </summary>
    public IEnumerable<string> GetAvailableResources() => fluentBundle.GetFuncEnumerable();

    public CultureInfo EffectiveCulture => fluentBundle.Culture;

    public void Dispose() => baseStore?.Dispose();
}
