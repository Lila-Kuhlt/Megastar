using System;
using ManagedBass;

namespace megastar.Game.Audio;

public class MicDevice(int deviceIndex)
{
    private int deviceIndex { get; set; } = deviceIndex;
    public AudioRingBuffer Buffer { get; } = new AudioRingBuffer(48000);
    public YinPitchDetector PitchDetector = new YinPitchDetector();
    public BassCaptureStream CaptureStream = new BassCaptureStream();

    private int recordChannel;

    public void Start()
    {
        CaptureStream.Start(deviceIndex);
    }

    public void OnAudioRecieved(ReadOnlySpan<float> samples)
    {
        Buffer.Write(samples);
    }

    public void Stop()
    {
        CaptureStream.Stop();
    }
}
