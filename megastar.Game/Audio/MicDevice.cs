using System;
using ManagedBass;

namespace megastar.Game.Audio;

public class MicDevice(int deviceIndex)
{
    public int DeviceIndex { get; set; } = deviceIndex;

    private int recordChannel;

    public void Start()
    {
        // Initialize recording device
        if (!Bass.RecordInit(-1))
            throw new Exception($"RecordInit failed: {Bass.LastError}");

        // 44100 Hz, mono, 16-bit PCM
        recordChannel = Bass.RecordStart(
            44100,
            1,
            BassFlags.RecordPause,
            RecordProc
        );

        if (recordChannel == 0)
            throw new Exception($"RecordStart failed: {Bass.LastError}");

        // start recording
        Bass.ChannelPlay(recordChannel);
    }

    private bool RecordProc(int handle, IntPtr buffer, int length, IntPtr user)
    {
        return true;
    }

    public void Stop()
    {
        Bass.ChannelStop(recordChannel);
        Bass.RecordFree();
    }
}
