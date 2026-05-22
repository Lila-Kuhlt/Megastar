using System;

namespace megastar.Game.Audio;

public class AudioRingBuffer(int size)
{
    private int writeIndex = 0;
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
            Span<float> targetBack = buffer.AsSpan().Slice(writeIndex, buffer.Length - writeIndex);
            Span<float> targetFront = buffer.AsSpan().Slice(0, samples.Length - buffer.Length - writeIndex);

            ReadOnlySpan<float> samplesFront = samples.Slice(0, buffer.Length - writeIndex);
            ReadOnlySpan<float> samplesBack = samples.Slice(writeIndex, samples.Length - buffer.Length - writeIndex);

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
            ReadOnlySpan<float> bufferBack = buffer.AsSpan().Slice(readIndex, buffer.Length - readIndex);
            ReadOnlySpan<float> bufferFront = buffer.AsSpan().Slice(0, destination.Length - buffer.Length - readIndex);

            Span<float> destinationFront = destination.Slice(0, buffer.Length - readIndex);
            Span<float> destinationBack = destination.Slice(writeIndex, destination.Length - buffer.Length - readIndex);

            bufferBack.CopyTo(destinationFront);
            bufferFront.CopyTo(destinationBack);
        }

        readIndex = (destination.Length + readIndex) % buffer.Length;
    }

    public void Clear()
    {
        writeIndex = 0;
        readIndex = 0;
        buffer = new float[size];
    }
}
