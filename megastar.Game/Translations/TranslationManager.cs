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
using osu.Framework.IO.Stores;

namespace megastar.Game.Translations;

public class TranslationManager : IResourceStore<FluentBundle>
{
    private readonly IResourceStore<byte[]> baseStore;
    private string selectedLanguage;

    /// <summary>
    /// Creates a new TranslationManager. With an optional default language.
    /// </summary>
    /// <param name="defaultLanguage">
    /// (optional) Language code of the language to use. If not set or language not available, uses the first available translation in the Translations directory.
    /// </param>
    public TranslationManager(string defaultLanguage = null)
    {
        var resourceAssembly = typeof(MegastarResources).Assembly;
        baseStore = new NamespacedResourceStore<byte[]>(new DllResourceStore(resourceAssembly), "Translations");
        selectedLanguage = defaultLanguage != null && GetAvailableResources().Contains(defaultLanguage) ? defaultLanguage : GetAvailableResources().First();
    }

    /// <summary>
    /// Updates the default language used for translations.
    /// </summary>
    /// <param name="language">
    /// Language code of the language to use. If language not available, keeps the previous language
    /// </param>
    public void SetDefaultLanguage(string language)
    {
        selectedLanguage = language;
    }

    /// <summary>
    ///     Passthrough method for <see cref="IReadBundle.GetAttrMessage" />, so there is less method chaining.
    /// </summary>
    /// <param name="language">The language to use.</param>
    /// <param name="msgWithAttr">The message with attribute to retrieve.</param>
    /// <param name="args">Optional arguments to format the message.</param>
    /// <returns>The attribute message from the read bundle.</returns>
    public string GetAttrMessage(string msgWithAttr, params (string, IFluentType)[] args)
    {
        return GetAttrMessage(selectedLanguage, msgWithAttr, args);
    }

    /// <summary>
    ///     Passthrough method for <see cref="IReadBundle.GetAttrMessage" />, so there is less method chaining. If possible use <see cref="GetAttrMessage(string, ValueTuple{string, IFluentType}[])"/>
    /// </summary>
    /// <param name="language">The language to use.</param>
    /// <param name="msgWithAttr">The message with attribute to retrieve.</param>
    /// <param name="args">Optional arguments to format the message.</param>
    /// <returns>The attribute message from the read bundle.</returns>
    public string GetAttrMessage(string language, string msgWithAttr, params (string, IFluentType)[] args)
    {
        try
        {
            return Get(language).GetAttrMessage(msgWithAttr, args);
        }
        catch (LinguiniException)
        {
            Console.Error.WriteLine($"[ERROR] Missing translation for key \"{msgWithAttr}\" in language {language}");
            return $"[MISSING] key \"{msgWithAttr}\" in language {language}";
        }
    }

    /// <summary>
    /// Retrieves the translation bundle for the default language.
    /// </summary>
    public FluentBundle Get()
    {
        return Get(selectedLanguage);
    }

    /// <summary>
    /// Retrieves the translation bundle for the default language asynchronously.
    /// </summary>
    public async Task<FluentBundle> GetAsync(CancellationToken cancellationToken = default)
    {
        return await GetAsync(selectedLanguage, cancellationToken);
    }

    /// <summary>
    /// Retrieves the translation bundle for a specific language. If possible use <see cref="Get()"/>
    /// </summary>
    /// <param name="language">
    /// Language code of the language to use. If language not available returns null.
    /// </param>
    public FluentBundle Get(string language)
    {
        if (!language.EndsWith(".ftl"))
            language += ".ftl";
        var data = baseStore.Get(language);

        var (bundle, errors) = LinguiniBuilder.Builder().CultureInfo(new CultureInfo(Regex.Replace(language, "\\.ftl$", ""))).AddResource(new StreamReader(new MemoryStream(data))).Build();

        if (errors != null && errors.Any())
        {
            errors.ForEach(e => Console.Error.WriteLine(e));
        }

        return bundle;
    }

    /// <summary>
    /// Retrieves the translation bundle for a specific language asynchronously. If possible use <see cref="GetAsync(CancellationToken)"/>
    /// </summary>
    /// <param name="language">
    /// Language code of the language to use. If language not available returns null.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token.
    /// </param>
    public async Task<FluentBundle> GetAsync(string language, CancellationToken cancellationToken = default)
    {
        if (!language.EndsWith(".ftl"))
            language += ".ftl";
        var data = await baseStore.GetAsync(language, cancellationToken);

        var (bundle, errors) = LinguiniBuilder.Builder().CultureInfo(new CultureInfo(Regex.Replace(language, "\\.ftl$", ""))).AddResource(new StreamReader(new MemoryStream(data))).Build();

        if (errors.Any())
        {
            errors.ForEach(e => Console.Error.WriteLine(e));
        }

        return bundle;
    }

    public Stream GetStream(string name) => baseStore.GetStream(name);

    /// <summary>
    /// Returns a list of all available languages by language code.
    /// </summary>
    public IEnumerable<string> GetAvailableResources() => baseStore.GetAvailableResources().Select(r => Regex.Replace(r, "\\.ftl$", ""));

    public void Dispose() => baseStore?.Dispose();
}
