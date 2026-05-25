using System;

namespace megastar.Game.Audio;

public class AudioRingBuffer(int size)
{
    public int writeIndex = 0;
    private int readIndex = 0;
    private float[] buffer = new float[size];

    public void Write(ReadOnlySpan<float> samples)
    {
        if (samples.Length <= buffer.Length - writeIndex)
        {
            Span<float> targetBuffer = buffer.AsSpan().Slice(writeIndex, samples.Length);
            samples.CopyTo(targetBuffer);
        }
        else
        {
            int tailSpace = buffer.Length - writeIndex;
            int wrapSpace = samples.Length - tailSpace;

            Span<float> targetBack = buffer.AsSpan().Slice(writeIndex, tailSpace);
            Span<float> targetFront = buffer.AsSpan().Slice(0, wrapSpace);

            ReadOnlySpan<float> samplesFront = samples.Slice(0, tailSpace);
            ReadOnlySpan<float> samplesBack = samples.Slice(tailSpace, wrapSpace);

            samplesFront.CopyTo(targetBack);
            samplesBack.CopyTo(targetFront);

        }

        writeIndex = (samples.Length + writeIndex) % buffer.Length;
    }

    public void ReadTo(Span<float> destination)
    {
        if (destination.Length <= buffer.Length - readIndex)
        {
            ReadOnlySpan<float> bufferSlice = buffer.AsSpan().Slice(readIndex, destination.Length);
            bufferSlice.CopyTo(destination);
        }
        else
        {
            int tailSamples = buffer.Length - readIndex;
            int wrapSamples = destination.Length - tailSamples;

            ReadOnlySpan<float> bufferBack = buffer.AsSpan().Slice(readIndex, tailSamples);
            ReadOnlySpan<float> bufferFront = buffer.AsSpan().Slice(0, wrapSamples);

            Span<float> destinationFront = destination.Slice(0, tailSamples);
            Span<float> destinationBack = destination.Slice(tailSamples, wrapSamples);

            bufferBack.CopyTo(destinationFront);
            bufferFront.CopyTo(destinationBack);
        }

        readIndex = (512 + readIndex) % buffer.Length;
    }

    public void Clear()
    {
        writeIndex = 0;
        readIndex = 0;
        Array.Clear(buffer);
    }
}
