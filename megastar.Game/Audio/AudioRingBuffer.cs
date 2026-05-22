using System;

namespace megastar.Game.Audio;

public class AudioRingBuffer(int size)
{
    private int writeIndex = 0;
    private int readIndex = 0;
    private float[] buffer =  new float[size];

    public void Write(float[] samples)
    {
        if (writeIndex + samples.Length <= buffer.Length)
        {
            Array.Copy(samples, 0, buffer, writeIndex, samples.Length);

            writeIndex += samples.Length;
        }
        else
        {
            int index = buffer.Length - writeIndex;
            Array.Copy(samples, 0, buffer, writeIndex, buffer.Length - writeIndex);
            writeIndex = 0;
            Array.Copy(samples, index, buffer, writeIndex, samples.Length - index);
            writeIndex += samples.Length - index;
        }
    }

    public void Clear()
    {
        writeIndex = 0;
        readIndex = 0;
        buffer = new float[size];
    }
}
