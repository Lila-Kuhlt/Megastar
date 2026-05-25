using System;

namespace megastar.Game.Audio;

public class YinPitchDetector
{
    private readonly int sampleRate;
    private readonly int frameSize;
    private readonly int minTau;
    private readonly int maxTau;

    private float[] difference;
    private float[] cumulativeMean;

    public float LastFrequency { get; private set; } = -1f;
    public float LastAperiodicity { get; private set; } = 1f;

    public YinPitchDetector()
    {
        this.sampleRate = 48000;
        this.frameSize = 2048;

        this.minTau = 80;
        this.maxTau = 1100;

        difference = new float[maxTau];
        cumulativeMean = new float[maxTau];
    }

    public float DetectPitch(ReadOnlySpan<float> samples)
    {
        if (samples.Length < frameSize)
            throw new ArgumentException(
                $"Buffer muss mindestens {frameSize} Samples enthalten.", nameof(samples));

        computeDifferenceFunction(samples);
        cumulativeMeanNormalizedDifference();

        int tauInt = absoluteThreshold();

        if (tauInt < 0)
        {
            LastFrequency = -1f;
            LastAperiodicity = 1f;
            return -1f;
        }

        float tauRefined = parabolicInterpolation(tauInt);
        float frequency = sampleRate / tauRefined;

        LastFrequency = frequency;
        LastAperiodicity = cumulativeMean[tauInt];

        return frequency;
    }

    private void computeDifferenceFunction(
        ReadOnlySpan<float> samples)
    {
        difference[0] = 1f;

        for (int tau = 1; tau < maxTau; tau++)
        {
            float sum = 0f;

            int limit = frameSize - tau;

            for (int i = 0; i < limit; i++)
            {
                float delta =
                    samples[i] - samples[i + tau];

                sum += delta * delta;
            }

            difference[tau] = sum;
        }
    }

    private void cumulativeMeanNormalizedDifference()
    {
        cumulativeMean[0] = 1f;
        double runningSum = 0.0;

        for (int tau = 1; tau < maxTau; tau++)
        {
            runningSum += difference[tau];
            if (runningSum > 0.0)
            {
                cumulativeMean[tau] = (float)(difference[tau] * tau/ runningSum);
            }
            else
            {
                cumulativeMean[tau] = 1f;
            }
        }
    }

    private int absoluteThreshold()
    {
        float threshold = 0.15f;
        for (int tau = 2; tau < maxTau; tau++)
        {
            if (tau < threshold)
            {
                while ((tau + 1 < maxTau) && cumulativeMean[tau + 1] < cumulativeMean[tau])
                {
                    tau++;
                }

                return tau;
            }
        }

        return -1;
    }

    private float parabolicInterpolation(int tau)
    {

        if (tau <= 0 || tau >= maxTau - 1)
            return tau;

        float s0 = cumulativeMean[tau - 1];
        float s1 = cumulativeMean[tau];
        float s2 = cumulativeMean[tau + 1];

        float denominator = 2f * (s0 - 2f * s1 + s2);
        if (MathF.Abs(denominator) < 1e-9f)
            return tau;

        float offset = (s0 - s2) / denominator;

        if (offset > 0.5f) offset = 0.5f;
        else if (offset < -0.5f) offset = -0.5f;

        return tau + offset;
    }
}
