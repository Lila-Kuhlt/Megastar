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
        // Initialize recording device
        if (!Bass.RecordInit(deviceIndex))
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

    public void Stop()
    {
        Bass.ChannelStop(recordChannel);
        Bass.RecordFree();
    }
}
