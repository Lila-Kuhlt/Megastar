using System;

namespace megastar.Game.Audio;

public class AudioRingBuffer(int size)
{
    private int writeIndex = 0;
    private int readIndex = 0;
    private float[] buffer =  new float[size];

    public void Write(ReadOnlySpan<float> samples)
    {

    }

    public float[] ReadRange(int howMuch)
    {
        float[] samples = new float[howMuch];
        if (readIndex + howMuch <= buffer.Length)
        {

            Array.Copy(buffer, readIndex, samples, 0, howMuch);
            readIndex += howMuch;
        }
        else
        {
            int index = buffer.Length - readIndex;
            Array.Copy(buffer, readIndex, samples, 0,index);
            readIndex = 0;
            Array.Copy(buffer, index, samples, readIndex, howMuch - index);
            readIndex += howMuch - index;
        }
        return samples;
    }

    public void Clear()
    {
        writeIndex = 0;
        readIndex = 0;
        buffer = new float[size];
    }
}
