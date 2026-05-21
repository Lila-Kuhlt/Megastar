using System;
using System.Collections.Generic;
using ManagedBass;

namespace megastar.Game.Audio;

public class MicManager
{
    private List<MicDevice> availableMics;

    public void AddMic(int deviceIndex)
    {
        MicDevice newMic = new MicDevice(deviceIndex);
        availableMics.Add(newMic);
    }

    public void RemoveMic(int deviceIndex)
    {
        try
        {
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
        foreach (MicDevice mic in availableMics)
        {
            mic.Start();
        }
    }

    public void StopAll()
    {
        foreach (MicDevice mic in availableMics)
        {
            mic.Stop();
        }
    }

}
