using System;

namespace megastar.Game.Audio;

public class MicDevice()
{
    private int deviceIndex { get; }
    private AudioRingBuffer buffer { get; } = new AudioRingBuffer(48000);
    private readonly YinPitchDetector pitchDetector = new YinPitchDetector();
    private readonly BassCaptureStream captureStream = new BassCaptureStream();
    private readonly PitchMedianFilter medianFilter =  new PitchMedianFilter();

    private float[] currentSamples = new float[2048];

    public MicDevice(int deviceIndex) : this()
    {
        this.deviceIndex = deviceIndex;
        captureStream.AudioReceived += OnAudioReceived;
    }

    public void Start()
    {
        captureStream.Start(deviceIndex);

        Console.WriteLine("started mic with index {0}", deviceIndex);
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

    public void ProcessAudioFrame()
    {
        buffer.ReadTo(currentSamples);

        float rawPitch = pitchDetector.DetectPitch(currentSamples);

        if (rawPitch > 0)
        {
            float smoothedPitch = medianFilter.Filter(rawPitch);
            Console.WriteLine(smoothedPitch);
        }
        else
        {
            Console.WriteLine(":(");
        }
    }
}
