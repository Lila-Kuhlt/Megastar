using System;
using ManagedBass;

namespace megastar.Game.Audio;

public class MicDevice()
{
    private int deviceIndex { get; }
    private AudioRingBuffer buffer { get; } = new AudioRingBuffer(48000);
    public YinPitchDetector PitchDetector = new YinPitchDetector();
    private readonly BassCaptureStream captureStream = new BassCaptureStream();


    public MicDevice(int deviceIndex) : this()
    {
        this.deviceIndex = deviceIndex;
        captureStream.AudioReceived += OnAudioReceived;
    }

    public void Start()
    {
        captureStream.Start(deviceIndex);
    }

    private void OnAudioReceived(
        ReadOnlySpan<float> samples)
    {
        buffer.Write(samples);
    }

    public void Stop()
    {
        captureStream.Stop();
        buffer.Clear();
    }

    public void Dispose()
    {
        captureStream.AudioReceived -= OnAudioReceived;

        buffer.Clear();
        captureStream.Stop();
    }
}
