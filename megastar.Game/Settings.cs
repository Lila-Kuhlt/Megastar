using System;
using osu.Framework.Bindables;
using osu.Framework.Configuration;
using osu.Framework.Platform;

namespace megastar.Game;

public enum GameDifficulty
{
    Kuhlant,
    Muuuuuhtig,
    Kuhtastrophal,
}


public enum GameSetting
{
    SoundVolume,
    Difficulty,
    LastIndexPath,
    WebAppActive,
    DuplicateItems
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
        SetDefault(GameSetting.Difficulty, GameDifficulty.Muuuuuhtig);
        SetDefault(GameSetting.LastIndexPath, Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
        SetDefault(GameSetting.WebAppActive, false);
        SetDefault(GameSetting.DuplicateItems, false);
    }

    // Expose Settings here
    public Bindable<int> SoundVolume => GetBindable<int>(GameSetting.SoundVolume);
    public Bindable<GameDifficulty> Difficulty => GetBindable<GameDifficulty>(GameSetting.Difficulty);
    public Bindable<string> LastIndexPath => GetBindable<string>(GameSetting.LastIndexPath);
    public Bindable<bool> WebAppActive => GetBindable<bool>(GameSetting.WebAppActive);
    public Bindable<bool> DuplicateItems => GetBindable<bool>(GameSetting.DuplicateItems);
    // Das ist extra, dass das per session immer false ist
    public Bindable<bool> WebAppStarted { get; } = new Bindable<bool>(false);
}
