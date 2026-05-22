using System;
using ManagedBass;

namespace megastar.Game.Audio;

public delegate void AudioReceivedHandler(
    ReadOnlySpan<float> samples);

public class BassCaptureStream
{
    public event AudioReceivedHandler? AudioReceived;
    private int recordChannel;

    private unsafe bool RecordProc(
        int handle,
        IntPtr buffer,
        int length,
        IntPtr user)
    {
        int sampleCount = length / sizeof(float);

        float* ptr = (float*)buffer;

        ReadOnlySpan<float> samples =
            new ReadOnlySpan<float>(ptr, sampleCount);

        AudioReceived?.Invoke(samples);

        return true;
    }

    public void Start(int deviceIndex)
    {
        if (!Bass.RecordInit(deviceIndex))
            throw new Exception($"RecordInit failed: {Bass.LastError}");

        recordChannel = Bass.RecordStart(
            48000,
            1,
            BassFlags.RecordPause,
            RecordProc
        );

        if (recordChannel == 0)
            throw new Exception($"RecordStart failed: {Bass.LastError}");

        // start recording
        Bass.ChannelPlay(recordChannel);
    }

    public void Stop()
    {
        if (recordChannel != 0)
        {
            Bass.ChannelStop(recordChannel);
            recordChannel = 0;
        }
        Bass.RecordFree();
    }
}
