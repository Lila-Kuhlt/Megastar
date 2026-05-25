using osu.Framework.Localisation;

namespace megastar.Game.Translations;

public class Language
{
    public Language(string code, LocalisableString name, LocalisationManager localisationManager)
    {
        Code = code;
        Name = name;
        manager = localisationManager;
    }

    public string Code { get; }
    public LocalisableString Name { get; }
    private LocalisationManager manager;

    public override string ToString()
    {
        return manager.GetLocalisedString(Name);
    }
};
