using osu.Framework.Bindables;
using osu.Framework.Configuration;
using osu.Framework.Platform;

namespace megastar.Game;

public enum GameSetting
{
    SoundVolume
}

public class Settings : IniConfigManager<GameSetting>
{
    private static Settings instance;

    public static void Initialize(Storage storage)
    {
        instance ??= new Settings(storage);
    }

    public static Settings GetSettings()
    {
        return instance ?? throw new System.InvalidOperationException("Settings must be initialized with a Storage host first.");
    }


    private Settings(Storage storage) : base(storage)
    {
    }
    protected override string Filename => "game.ini";

    protected override void InitialiseDefaults()
    {
        SetDefault(GameSetting.SoundVolume, 100, 0, 100); // Key, Default, Min, Max
    }

    // Expose Settings here
    public Bindable<int> SoundVolume => GetBindable<int>(GameSetting.SoundVolume);
}
