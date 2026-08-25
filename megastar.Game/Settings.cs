using System;
using System.Collections.Generic;
using ManagedBass;
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
    public static readonly Colour4 MAIN = Colour4.FromHex("#C95792");
    public static readonly Colour4 SECOND = Colour4.FromHex("#7C4585");
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
    DuplicateItems,

    // NEW: Microphone settings
    MicrophoneCount,
    MicrophoneDevices
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
        SetDefault(GameSetting.SoundVolume, 100, 0, 100);
        SetDefault(GameSetting.Difficulty, GameDifficulty.Muuuuuhtig);
        SetDefault(GameSetting.LastIndexPath, Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
        SetDefault(GameSetting.WebAppActive, false);
        SetDefault(GameSetting.DuplicateItems, false);

        SetDefault(GameSetting.MicrophoneCount, 1, 1, 4);
        SetDefault(GameSetting.MicrophoneDevices, "Default");
    }


    public Bindable<int> SoundVolume => GetBindable<int>(GameSetting.SoundVolume);
    public Bindable<GameDifficulty> Difficulty => GetBindable<GameDifficulty>(GameSetting.Difficulty);
    public Bindable<string> LastIndexPath => GetBindable<string>(GameSetting.LastIndexPath);
    public Bindable<bool> WebAppActive => GetBindable<bool>(GameSetting.WebAppActive);
    public Bindable<bool> DuplicateItems => GetBindable<bool>(GameSetting.DuplicateItems);


    public Bindable<int> MicrophoneCount => GetBindable<int>(GameSetting.MicrophoneCount);

    // Raw string containing delimited device names (e.g., "Device1|Device2|Device3")
    //This is as osu! does not have an easy way to store an array
    public Bindable<string> MicrophoneDevicesRaw => GetBindable<string>(GameSetting.MicrophoneDevices);

    /// <summary>
    /// Gets the selected device name for a specific microphone index.
    /// </summary>
    public string GetMicrophoneDevice(int index)
    {
        var devices = MicrophoneDevicesRaw.Value.Split('|');
        return index < devices.Length ? devices[index] : "Default";
    }

    /// <summary>
    /// Sets the device name for a specific microphone index and saves it to the delimited string.
    /// </summary>
    public void SetMicrophoneDevice(int index, string deviceName)
    {
        var devices = MicrophoneDevicesRaw.Value.Split('|');
        var deviceList = new List<string>(devices);

        // Pad the list with "Default" if we are accessing a higher index than currently stored
        while (deviceList.Count <= index)
            deviceList.Add("Default");

        deviceList[index] = deviceName;
        MicrophoneDevicesRaw.Value = string.Join("|", deviceList);
    }


    /// <summary>
    /// Returns the index of the input device, that Bass expects as input
    /// </summary>
    /// <param name="targetDeviceName"></param>
    /// <returns>The index, or -1 if none was found</returns>
    public int GetBassDeviceIndexByName(string targetDeviceName)
    {
        int index = 0;

        while (Bass.RecordGetDeviceInfo(index, out DeviceInfo info))
        {
            if (info.Name == targetDeviceName)
            {
                return index;
            }
            index++;
        }

        return -1;
    }
}
