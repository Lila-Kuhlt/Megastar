using System;
using osu.Framework.Bindables;
using osu.Framework.Configuration;
using osu.Framework.Graphics;
using osu.Framework.Platform;

namespace megastar.Game;

public enum GameDifficulty
{
    Kuhlant,
    Muuuuuhtig,
    Kuhtastrophal,
}

public static class StandardColours
{
    //Pink
    public static readonly Colour4 MAIN = Colour4.FromHex("#C95792");
    //Purple
    public static readonly Colour4 SECOND = Colour4.FromHex("#7C4585");
    //Yellow
    public static readonly Colour4 THIRD = Colour4.FromHex("#F8B55F");
    public static readonly Colour4 TEXT = Colour4.White;
    public static readonly Colour4 BACKGROUND_TEXT = Colour4.DarkGray;
    public static readonly Colour4 BACKGROUND = Colour4.DeepPink;
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
        return instance ?? throw new InvalidOperationException("Settings must be initialized with a Storage host first.");
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
}
