using System;
using System.Runtime.InteropServices;
using ManagedBass;
using PitchTracking;

namespace megastar.Game.pitch
{
    public class MicrophonePitchTracker : IDisposable
    {
        private readonly PitchTracker pitchTracker;
        private int recordStream;
        private float[] audioBuffer;
        private int playerIndex = 1;

        // We must keep a strong reference to the callback delegate to prevent the GC from collecting it
        private RecordProcedure _recordProcedure;

        public event Action<PitchRecord, int> PitchDetected;

        public MicrophonePitchTracker(int playerIndex = 1)
        {
            pitchTracker = new PitchTracker();
            this.playerIndex = playerIndex;
            pitchTracker.PitchDetected += record => PitchDetected?.Invoke(record, playerIndex);
        }

        /// <summary>
        /// Starts the pitch tracker with the given device as input. If no input device is selected, the default (-1) is choosen
        /// </summary>
        /// <param name="deviceIndex"></param>
        /// <returns></returns>
        public bool Start(int deviceIndex = -1)
        {
            if (!Bass.RecordInit(deviceIndex))
            {
                if (Bass.LastError != Errors.Already)
                    return false;
            }

            _recordProcedure = Procedure;

            recordStream = Bass.RecordStart(44100, 1, BassFlags.RecordPause | BassFlags.Float, 50, _recordProcedure);

            if (recordStream == 0) return false;

            Bass.ChannelPlay(recordStream);
            return true;
        }

        public void Stop()
        {
            if (recordStream != 0)
            {
                Bass.ChannelStop(recordStream);
                recordStream = 0;
            }
        }

        private bool Procedure(int handle, IntPtr buffer, int length, IntPtr user)
        {
            int floatCount = length / 4;

            if (audioBuffer == null || audioBuffer.Length < floatCount)
                audioBuffer = new float[floatCount];

            // Copy unmanaged memory to C# float array
            Marshal.Copy(buffer, audioBuffer, 0, floatCount);

            // Process the audio data
            pitchTracker.ProcessBuffer(audioBuffer);

            return true;
        }

        public void Dispose()
        {
            Stop();
            Bass.RecordFree();
        }
    }
}
