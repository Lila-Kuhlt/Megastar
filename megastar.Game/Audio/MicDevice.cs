using System;
using ManagedBass;

namespace megastar.Game.Audio;

public class MicDevice(int deviceIndex)
{
    private int deviceIndex { get; set; } = deviceIndex;
    private AudioRingBuffer buffer { get; } = new AudioRingBuffer(48000);
    public YinPitchDetector PitchDetector = new YinPitchDetector();
    private readonly BassCaptureStream captureStream = new BassCaptureStream();

    private int recordChannel;

    public void Start()
    {
        captureStream.Start(deviceIndex);
    }

    public void OnAudioRecieved(ReadOnlySpan<float> samples)
    {
        buffer.Write(samples);
    }

    public void Stop()
    {
        captureStream.Stop();
    }
}
