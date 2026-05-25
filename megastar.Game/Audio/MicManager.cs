using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using ManagedBass;

namespace megastar.Game.Audio;

public class MicManager
{
    private readonly List<MicDevice> availableMics = new();

    private Thread? processingThread;
    private bool running;

    public void AddMic(int deviceIndex)
    {
        availableMics.Add(new MicDevice(deviceIndex));
    }

    public void RemoveMic(int deviceIndex)
    {
        try
        {
            availableMics[deviceIndex].Dispose();
            availableMics.RemoveAt(deviceIndex);
        }
        catch (ArgumentOutOfRangeException e)
        {
            Console.WriteLine("Device with number " + deviceIndex + " not found!");
        }
    }

    public List<MicDevice> GetAvailableMics()
    {
        return availableMics;
    }

    public void StartAll()
    {
        running = true;
        foreach (MicDevice mic in availableMics)
        {
            mic.Start();
        }

        processingThread = new Thread(processLoop);
        processingThread.Start();
    }

    public void StopAll()
    {
        running = false;
        processingThread?.Join();

        foreach (MicDevice mic in availableMics)
        {
            mic.Stop();
        }
    }

    private void processLoop()
    {
        Stopwatch stopwatch = Stopwatch.StartNew();

        const double interval_ms = 11;

        while (running)
        {
            long start = stopwatch.ElapsedMilliseconds;

            foreach (MicDevice mic in availableMics)
                mic.ProcessAudioFrame();

            long elapsed = stopwatch.ElapsedMilliseconds - start;

            int sleep = (int)(interval_ms - elapsed);

            if (sleep > 0)
                Thread.Sleep(sleep);
        }
    }

}
