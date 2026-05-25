namespace megastar.Game.Audio;

using System;

public class PitchMedianFilter
{
    private readonly int windowSize;
    private readonly float[] buffer;
    private readonly float[] sortBuffer;
    private int writeIndex = 0;
    private int count = 0;

    public PitchMedianFilter(int windowSize = 5)
    {
        this.windowSize = windowSize;
        this.buffer = new float[windowSize];
        this.sortBuffer = new float[windowSize];
    }

    public float Filter(float newPitch)
    {
        buffer[writeIndex] = newPitch;
        writeIndex = (writeIndex + 1) % windowSize;
        if (count < windowSize) count++;

        if (count < windowSize) return newPitch;

        Array.Copy(buffer, sortBuffer, windowSize);
        Array.Sort(sortBuffer);

        return sortBuffer[windowSize / 2];
    }
}
